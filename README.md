# TodoServer

This is a very minimal to-do list project.

## Built Release Usage (Ubuntu Server)
(This was written on August 6th, 2024)


Extract the .zip from the Release. It should be in the format `TodoServerRelease-x.y.z.zip`, where `x.y.z` is the TodoServer version.

### Downloading .NET
(If you wish to use on Windows, you can go to the .NET website and download the runtimes for whatever version is specified by the .NET version in the associated release).

Run these commands as a user with sudo permissions:
```# Get OS version info which adds the $ID and $VERSION_ID variables
source /etc/os-release

# Download Microsoft signing key and repository
wget https://packages.microsoft.com/config/$ID/$VERSION_ID/packages-microsoft-prod.deb -O packages-microsoft-prod.deb

# Install Microsoft signing key and repository
sudo dpkg -i packages-microsoft-prod.deb

# Clean up
rm packages-microsoft-prod.deb

# Update packages
sudo apt update

# Download the appropriate .NET version (specified in the release, where "a.b" is the .NET version; e.g. to install .NET 8.0, you would run "sudo apt install -y dotnet-sdk-8.0")
sudo apt install -y dotnet-sdk-a.b
```

### Downloading the release files
```
# Download git release (make sure to change x.y.z to the correct version number)
wget https://github.com/CoderUser141/TodoServer/releases/latest/download/TodoServerRelease-x.y.z.zip

# Unzip the zip package (again, change x.y.z to match version number)
unzip TodoServerRelease-x.y.z.zip
```

You should now have two folders in the directory you unzipped them in: `todo-server-backend` and `todo-server-frontend`.

Determine what port the backend runs on (this is typically 5000) by running the associated dll:
```
dotnet todo-server-backend/TodoServerBackend.dll
```
This will usually say:
```
info: Microsoft.Hosting.Lifetime[14]                                                       Now listening on: http://localhost:5000
...more output...
```
If this number is NOT 5000, make a note of it.

###  Setting up the frontend with NGINX

Copy `todo-server-frontend` to `/var/www/` by running:
```
sudo cp -R todo-server-frontend /var/www
```

Set proper permissions for NGINX with:
```
sudo chown -R www-data:www-data /var/www/todo-server-frontend
sudo chmod -R 755 /var/www/todo-server-frontend
```

This will ensure NGINX has proper permissions to read from the website frontend.

If you do not remember installing NGINX, check with:
```
nginx -v
```
If there is no error, then you have NGINX. Otherwise, install it with:
```
sudo apt update && sudo apt install -y nginx
```
Now, create a configuration file for the frontend. This will handle requests to the base URL and proxy requests to the backend. I prefer to make the file directly in `sites-enabled`, but others prefer creating the file first in `sites-available` and then symbolically linking it afterwards.
<br>
You can use vim or nano or something else, I prefer nano:
```
sudo nano /etc/nginx/sites-enabled/todosite.conf
```
Paste in this configuration (I use this personally, and 4673 port can be changed for any open port on the machine), and save the file:
```
server{
    listen 0.0.0.0:4673;
    location /{
        root /var/www/todo-server-frontend/browser;
        index index.html;
    }
    location /todo/{
        proxy_pass http://localhost:5000/todo/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
    error_log /var/www/error.log warn;
}
```
<b>NOTE:</b> If the port number for the backend was different than 5000, <b>change it here:</b> i.e. it would now be `proxy_pass http://localhost:yourportnumber/todo/`

Test that the configuration works by running:
```
sudo nginx -t
```
If this is successful, then restart NGINX:
```
sudo systemctl restart nginx
```
Then ensure that the website pops up by going to the IP address of the server: xxx.xxx.xxx.xxx:4673 (or whatever port you used).

Additionally, if you would like to test the backend, run:
```
dotnet todo-server-backend/TodoServerBackend.dll
```
Ensure that you can add/delete/read todo tasks.

### Running the backend as a background service (daemon)

Copy the backend files to `/var/www` (similar to the frontend steps above.):
```
sudo cp -R todo-server-backend /var/www
```
You might be tempted to reuse the commands from earlier, but using `chmod -R 755` WILL break the backend (it needs read permissions to access the database). Instead, run:
```
sudo chown -R www-data:www-data /var/www/todo-server-backend
sudo chmod -R 777 /var/www/todo-server-backend
```
Now, create a file in `/etc/systemd/system/`:
```
sudo nano /etc/systemd/system/todositebackend.service
```
Paste in the following configuration, and save the file:
```
[Unit]                                                                                                                  Description=Backend service for TODO site. Built on ASP.NET, and accessed through frontend Angular at /var/www/todo-server-frontend.

[Service]
# Specifies working directory
WorkingDirectory=/var/www/todo-server-backend

# Specifies the command to run
ExecStart=/usr/share/dotnet/dotnet /var/www/todo-server-backend/TodoServerBackend.dll

Restart=always
 # Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT

# For debugging/logs
SyslogIdentifier=todositebackend

# User to run as
User=www-data

# Environment variables needed for .NET
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
                                                 
[Install]
WantedBy=multi-user.target
```
Then, refresh the daemon list:
```
sudo systemctl daemon-reload
```
Enable and start the service:
```
sudo systemctl enable todositebackend && sudo systemctl start todositebackend
``` 
Check the service's status with
```
systemctl status todositebackend
```
If there is an error somewhere, view the full logs here:
```
sudo journalctl -t todositebackend
```
To stop the service:
```
sudo systemctl stop todositebackend
```

You should now have a running server.

## Source Building
Ensure you have Angular installed (with the version matching the release you have downloaded). (I may add a tutorial here later).

Navigate to where you have downloaded the source `.zip` file and extract it.
Navigate to `TodoServerFrontend`. Run:
```
ng build
```
This should make a new folder called `dist` in the same directory. Within it will be a folder called `todo-server-frontend`. This is the same folder that is used in the Release package, and you can release this on Ubuntu Server using the same steps above (minus extracting the latest release).

For the backend, you have two options (that I know of):
### Visual Studio
In Visual Studio, open the project file (TodoServerBackend.csproj). Then, right-click on the project in Solution Explorer and click "publish". This will create a new folder in `(rootDirectoryOfSolution)/TodoServer/TodoServerBackend/bin/Release/net\<version\>/` called `publish`. Copy the files in the `publish` folder to a new folder called `todo-server-backend`. This will be the same folder used in the Release package.

### Command line
In `(rootDirectoryOfSolution)/TodoServer/TodoServerBackend`, run
```
dotnet publish
```
As above, it will create a new folder in `(rootDirectoryOfSolution)/TodoServer/TodoServerBackend/bin/Release/net\<version\>/` called `publish`. Copy the files in the `publish` folder to a new folder called `todo-server-backend`. This will be the same folder used in the Release package.

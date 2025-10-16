MAKE_DIR = $(PWD)

FRONTEND_DIR		:=	$(MAKE_DIR)/TodoServerFrontend
FRONTEND_BUILD_DIR	:=	$(FRONTEND_DIR)/dist/
FRONTEND_SRC_DIR	:=	$(FRONTEND_DIR)/src
FRONTEND_SRC_FILES	:=	$(shell find $(FRONTEND_SRC_DIR) -name "*.ts") $(shell find $(FRONTEND_SRC_DIR) -name "*.html") $(shell find $(FRONTEND_SRC_DIR) -name "*.css") $(shell find $(FRONTEND_SRC_DIR) -name "*.json") $(shell find $(FRONTEND_SRC_DIR) -name "*.config") $(shell find $(FRONTEND_SRC_DIR) -name "*.esproj") $(shell find $(FRONTEND_SRC_DIR) -name "*.ts") $(shell find $(FRONTEND_SRC_DIR) -name "*.ico")  

BACKEND_DIR			:=	$(MAKE_DIR)/TodoServerBackend
BACKEND_BUILD_DIR	:=	$(BACKEND_DIR)/bin/Debug/net8.0
BACKEND_PUBLISH_DIR	:=	$(BACKEND_DIR)/bin/Release/net8.0/publish
BACKEND_FILES		:=	$(shell find $(FRONTEND_SRC_DIR) -name "*.cs") $(shell find $(FRONTEND_SRC_DIR) -name "*.db") $(shell find $(FRONTEND_SRC_DIR) -name "*.csproj") $(shell find $(FRONTEND_SRC_DIR) -name "*.json")   


FULL_BUILD_DIR		:=	$(MAKE_DIR)/output

.PHONY: frontendpublish
frontend: $(FRONTEND_SRC_FILES)
	cd $(FRONTEND_DIR); ng build; cd $(FRONTEND_BUILD_DIR); mkdir TodoServerFrontendD; cp -R todo-server-frontend/* TodoServerFrontendD/; rm -r $(FULL_BUILD_DIR)/TodoServerFrontendD; mv -u TodoServerFrontendD $(FULL_BUILD_DIR)/

.PHONY: backendpublish
backendpublish: $(BACKEND_FILES)
	cd $(BACKEND_DIR); dotnet publish; cd $(BACKEND_PUBLISH_DIR)/..; mkdir TodoServerBackendD; cp -R publish/* TodoServerBackendD/; rm -r $(FULL_BUILD_DIR)/TodoServerBackendD; mv -u TodoServerBackendD $(FULL_BUILD_DIR)/

.PHONY: backend
backend: $(BACKEND_FILES)
	cd $(BACKEND_DIR); dotnet build;

all: $(BACKEND_FILES) $(FRONTEND_SRC_FILES)
	cd $(FRONTEND_DIR); ng build; cd $(FRONTEND_BUILD_DIR); mkdir TodoServerFrontendD; cp -vaR todo-server-frontend/* TodoServerFrontendD; rm -r $(FULL_BUILD_DIR)/TodoServerFrontendD; mv -u TodoServerFrontendD $(FULL_BUILD_DIR)/
	cd $(BACKEND_DIR); dotnet publish; cd $(BACKEND_PUBLISH_DIR)/..; mkdir TodoServerBackendD; cp -R publish/* TodoServerBackendD/; rm -r $(FULL_BUILD_DIR)/TodoServerBackendD; mv -u TodoServerBackendD $(FULL_BUILD_DIR)/
	scp -rv $(FULL_BUILD_DIR)/ aethelhelm@172.26.182.221:TodoServer/
	ssh aethelhelm@172.26.182.221 ""  


.PHONY: clean
clean:
	rm $(FULL_BUILD_DIR)/*

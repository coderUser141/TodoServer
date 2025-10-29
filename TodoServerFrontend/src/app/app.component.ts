import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import * as moment from 'moment';
import { TaskRecordOutputModel, TaskRecordInputModel, TimeSyncModel, TaskCompletionStatus} from './app.definitions';



@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
	public OutputTaskList: TaskRecordOutputModel[] = [];
	public OutputTaskProjectIDList = new Map<number, string>();
	public OutputProjectCategoryIDList = new Map<number, string>();

	public FormTaskDeadline: string = "";
	public FormTaskName: string = "";
	public FormTaskDetails: string = "";
	public FormTaskCustomization: string = "";
	public FormTaskGrade: string = "0";
	public FormTaskWeight: string = "0";
	public FormTaskProjectID: string = "";
	public FormProjectCategoryID: string = "";
	public FormTaskCompletionStatus: TaskCompletionStatus = TaskCompletionStatus.NotStarted;

	public FormIncludeTime: boolean = false;
	
	public TCS = TaskCompletionStatus;
	
	public FormID: string = "";
	public FormIDEnd: string = "";

	backendroute: string = '/todo';
	offset: number = 0;

	



	constructor(private http: HttpClient) { }

	ngOnInit() {
		this.getTodoList();
		this.getTimeOffset();
		//console.log(this.OutputTaskList);
		//console.log(this.OutputTaskProjectIDList);
		//console.log(this.OutputProjectCategoryIDList);
	}

	populateIDLists(){
		//console.log("lala");
		for(let model of this.OutputTaskList){
			//console.log(model);
			this.OutputTaskProjectIDList.set(model.outputTaskProjectID, model.outputTaskProjectName);
			this.OutputProjectCategoryIDList.set(model.outputProjectCategoryID, model.outputProjectCategoryName);
		}
	}

	//forgive me, future me.
	//this functions takes the name and transforms it into the id IN-PLACE
	updateID(name: string, map: Map<number, string>): number{
		for(let entry of map.entries()){
			if(name == entry[1])return entry[0];
		}
		return -1;
	}


	updateCompletionStatus(status: TaskCompletionStatus): void{
		this.FormTaskCompletionStatus = status;
		//alert(this.FormTaskCompletionStatus);
	}

	decodeCompletionStatus(status: number): string{
		try{
			return TaskCompletionStatus[status].toString();
		}catch{
			return "";
		}
	}



	/**		GET METHODS		**/

	getTodoList(): void {
		this.http.get<TaskRecordOutputModel[]>(`${this.backendroute}/task`).subscribe({
			next: (x) => {
				let output: TaskRecordOutputModel[] = [];

				for (let model of x) {
					
					output.push(model);
				}
				this.OutputTaskList = output;
				//console.log(output);
				this.populateIDLists();
			},
			error: (x) => {
				console.error(x);
			}
		});
	}

	getTodoWithID() {
		this.OutputTaskList = [];
		if (this.FormIDEnd != "") {

			this.http.get<TaskRecordOutputModel[]>(`${this.backendroute}/task/${this.FormID}/${this.FormIDEnd}`).subscribe({
				next: (ReceivedTaskList) => {
					let temp: TaskRecordOutputModel[] = [];

					for (let TaskModel of ReceivedTaskList) {
						temp.push(TaskModel);
					}

					this.OutputTaskList = temp;
					this.populateIDLists();
				},
				error: (x) => {
					console.error(x);
				}
			});

		} else {

			this.http.get<TaskRecordOutputModel>(`${this.backendroute}/task/${this.FormID}`).subscribe({
				next: (ReceivedTaskList) => {
					this.OutputTaskList = []; //clear
					this.OutputTaskList.push(ReceivedTaskList);
					this.populateIDLists();
				},
				error: (x) => {
					console.error(x);
				}
			});

		}
	}

	getTimeOffset(){
		//Date.getTimezoneOffset gets timezone offset in minutes
		//suppose z = offset
		//the representation is GMT-z
		//i.e. GMT-5 is 300, and GMT+1 is -60
		//however, ReceivedTimeSync comes from zoneInfo.BaseUtcOffset.Hours represents GMT-5 as -300 GMT+1 as 60
		//THEY'RE OPPOSITE RAHHH

		let date: Date = new Date(); //get current time

		this.http.get<TimeSyncModel>(`${this.backendroute}/time/${date.getTimezoneOffset()}`).subscribe({
			next: (ReceivedTimeSync) => {
				this.offset = ReceivedTimeSync.serverOffset - ReceivedTimeSync.clientOffset;
				console.log(`serverOffset: ${ReceivedTimeSync.serverOffset}, clientOffset: ${ReceivedTimeSync.clientOffset}`)
				console.log(`You are ${this.offset} minutes behind the server (in regards to timezone)`);
			},
			error: (x) => {
				console.error(x);
			}
			
		});
	}


	updateTodo(){
		let tempPName: string = this.FormTaskProjectID;
		let tempCName: string = this.FormProjectCategoryID;

		this.FormTaskProjectID = this.updateID(this.FormTaskProjectID, this.OutputTaskProjectIDList).toString();

		this.FormProjectCategoryID = this.updateID(this.FormProjectCategoryID, this.OutputProjectCategoryIDList).toString();
		
		console.log(`${this.FormTaskProjectID}, ${this.FormProjectCategoryID}, ${this.FormTaskCompletionStatus}, ${this.FormTaskDeadline}`);
		
		let noChange: string = this.TCS.NoChange.toString();
		
		let task: TaskRecordInputModel = {
			InputTaskName: this.FormTaskName,
			InputTaskDetails: this.FormTaskDetails,
			InputTaskDeadline: this.FormTaskDeadline,
			InputTaskCustomization: this.FormTaskCustomization,
			InputTaskGrade: this.FormTaskGrade.toString(),
			InputTaskWeight: this.FormTaskWeight.toString(),
			InputTaskProjectName: tempPName,
			InputTaskProjectID: this.FormTaskProjectID,
			InputProjectCategoryName: tempCName,
			InputProjectCategoryID: this.FormProjectCategoryID,
			InputTaskCompletionStatus: this.FormTaskCompletionStatus.toString()
		}

		console.log(JSON.stringify(task))

		if(this.FormIDEnd != ""){
			this.http.patch(`${this.backendroute}/patch/${this.FormID}/${this.FormIDEnd}/${noChange}`, task).subscribe({
				next: (x) => {
					//alert("Added succesfully");
					console.log(x)
					this.getTodoList();
					this.FormTaskProjectID = tempPName;
					this.FormProjectCategoryID = tempCName;
				},
				error: (x) => {
					//console.log("lalal");
					//console.log(x);
					console.error(x);
					this.getTodoList();
					this.FormTaskProjectID = tempPName;
					this.FormProjectCategoryID = tempCName;
				}
			});
		}else{
			this.http.patch(`${this.backendroute}/patch/${this.FormID}/${noChange}`, task).subscribe({
				next: (x) => {
					//alert("Added succesfully");
					console.log(x)
					this.getTodoList();
					this.FormTaskProjectID = tempPName;
					this.FormProjectCategoryID = tempCName;
				},
				error: (x) => {
					//console.log("lalal");
					//console.log(x);
					console.error(x);
					this.getTodoList();
					this.FormTaskProjectID = tempPName;
					this.FormProjectCategoryID = tempCName;
				}
			});
		}
	}

	addTodo() {
		//let deadlinetemp = moment.default(this.FormTaskDeadline);



		//this is so cursed...  i'm transforming the name to an id right before posting it, then transforming it back for ui's sake lmaooo
		let tempPName: string = this.FormTaskProjectID;
		let tempCName: string = this.FormProjectCategoryID;

		this.FormTaskProjectID = this.updateID(this.FormTaskProjectID, this.OutputTaskProjectIDList).toString();

		this.FormProjectCategoryID = this.updateID(this.FormProjectCategoryID, this.OutputProjectCategoryIDList).toString();
		
		console.log(`${this.FormTaskProjectID}, ${this.FormProjectCategoryID}, ${this.FormTaskCompletionStatus}, ${this.FormTaskDeadline}`);
		

		let task: TaskRecordInputModel = {
			InputTaskName: this.FormTaskName,
			InputTaskDetails: this.FormTaskDetails,
			InputTaskDeadline: this.FormTaskDeadline,
			InputTaskCustomization: this.FormTaskCustomization,
			InputTaskGrade: this.FormTaskGrade.toString(),
			InputTaskWeight: this.FormTaskWeight.toString(),
			InputTaskProjectName: tempPName,
			InputTaskProjectID: this.FormTaskProjectID,
			InputProjectCategoryName: tempCName,
			InputProjectCategoryID: this.FormProjectCategoryID,
			InputTaskCompletionStatus: this.FormTaskCompletionStatus.toString()
		}

		console.log(JSON.stringify(task))

		this.http.post(this.backendroute+"/post", task).subscribe({
			next: (x) => {
				//alert("Added succesfully");
				console.log(x)
				this.getTodoList();
				this.FormTaskProjectID = tempPName;
				this.FormProjectCategoryID = tempCName;
			},
			error: (x) => {
				//console.log("lalal");
				//console.log(x);
				console.error(x);
				this.getTodoList();
				this.FormTaskProjectID = tempPName;
				this.FormProjectCategoryID = tempCName;
			}
		});

	}

	deleteTodo() {
		if (this.FormIDEnd != "") {
			
			this.http.delete(this.backendroute + "/" + this.FormID + "/" + this.FormIDEnd).subscribe({
				next: () => {
					//alert("Deleted succesfully");
					this.getTodoList();
				},
				error: (x) => {
					this.getTodoList();
					console.error(x);
				}
			});

		} else {

			this.http.delete(this.backendroute + "/" + this.FormID).subscribe({
				next: () => {
					//alert("Deleted succesfully");
					this.getTodoList();
				},
				error: (x) => {
					this.getTodoList();
					console.error(x);
				}
			});

		}
	}

	getUniqueID() {
		this.http.get<number>(this.backendroute + "/uid").subscribe({
			next: (x) => {
				alert(x);
			},
			error: (x) => {
				console.error(x);
			}
		});
		}

    sortByDeadlineDate() {
        /*this.OutputTaskList.sort(function (a, b): number {
            return moment.default(a.outputTaskDeadline).toDate().getTime() - moment.default(b.outputTaskDeadline).toDate().getTime();
        });*/
    }

}

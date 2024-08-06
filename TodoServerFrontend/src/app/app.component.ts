import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import * as moment from 'moment';


interface TaskRecordOutputModel {
	id: number;
	taskName: string;
	taskDetails: string;
	taskDeadline: string;
	taskCreationDate: string;
}

interface TaskRecordInputModel {
	taskName: string;
	taskDetails: string;
	taskDeadline: string;
}


@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
	public tasks: TaskRecordOutputModel[] = [];
	public date: Date = new Date();
	public taskname: string = "";
	public taskdetails: string = "";
	public ID: string = "";
	public IDEnd: string = "";
	backendroute: string = '/todo';


	constructor(private http: HttpClient) { }

	ngOnInit() {
		this.getTodoList();
	}


	getTodoList() {
		this.http.get<TaskRecordOutputModel[]>(this.backendroute).subscribe({
			next: (x) => {
				let temp: TaskRecordOutputModel[] = [];
				for (let model of x) {
					let deadline = moment.default(model.taskDeadline).local();
					model.taskDeadline = deadline.format("ddd DD MMM yyyy HH:mm:ss");

					let creation = moment.default(model.taskCreationDate).local();
					model.taskCreationDate = creation.format("ddd DD MMM yyyy HH:mm:ss");
					temp.push(model);
				}
				this.tasks = temp;
			},
			error: (x) => {
				console.error(x);
			}
		});
	}

	addTodo() {
		let deadlinetemp = moment.default(this.date);
		let task: TaskRecordInputModel = {
			taskName: this.taskname,
			taskDeadline: deadlinetemp.format(),
			taskDetails: this.taskdetails
		}

		this.http.post(this.backendroute+"/post", task).subscribe({
			next: () => {
				//alert("Added succesfully");
				this.getTodoList();
			},
			error: (x) => {
				console.error(x);
			}
		});


	}

	getTodoWithID() {
		this.tasks = [];
		if (this.IDEnd != "") {
			this.http.get<TaskRecordOutputModel[]>(this.backendroute + "/" + this.ID + "/" + this.IDEnd).subscribe({
				next: (x) => {
					let temp: TaskRecordOutputModel[] = [];
					for (let model of x) {
						let deadline = moment.default(model.taskDeadline).local();
						model.taskDeadline = deadline.format("ddd DD MMM yyyy HH:mm:ss");

						let creation = moment.default(model.taskCreationDate).local();
						model.taskCreationDate = creation.format("ddd DD MMM yyyy HH:mm:ss");
						temp.push(model);
					}
					this.tasks = temp;
				},
				error: (x) => {
					console.error(x);
				}
			});
		} else {
			this.http.get<TaskRecordOutputModel>(this.backendroute + "/" + this.ID).subscribe({
				next: (model) => {
					let deadline = moment.default(model.taskDeadline).local();
					model.taskDeadline = deadline.format("ddd DD MMM yyyy HH:mm:ss");

					let creation = moment.default(model.taskCreationDate).local();
					model.taskCreationDate = creation.format("ddd DD MMM yyyy HH:mm:ss");

					this.tasks.push(model);
				},
				error: (x) => {
					console.error(x);
				}
			});
		}
	}

	deleteTodo() {
		if (this.IDEnd != "") {
			this.http.delete(this.backendroute + "/" + this.ID + "/" + this.IDEnd).subscribe({
				next: () => {
					//alert("Deleted succesfully");
					this.getTodoList();
				},
				error: (x) => {
					console.error(x);
				}
			});
		} else {
			this.http.delete(this.backendroute + "/" + this.ID).subscribe({
				next: () => {
					//alert("Deleted succesfully");
					this.getTodoList();
				},
				error: (x) => {
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
        this.tasks.sort(function (a, b): number {
            return moment.default(a.taskDeadline).toDate().getTime() - moment.default(b.taskDeadline).toDate().getTime();
        });
    }

}

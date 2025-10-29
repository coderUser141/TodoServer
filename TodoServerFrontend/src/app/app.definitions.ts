/*
DO NOT CHANGE THE FIRST LETTER CASING.
If the frontend list does NOT work, check the JSON Output by using {{ Task | json }}
*/
export interface TaskRecordOutputModel {
	id: number;
	outputTaskName: string;
	outputTaskDetails: string;
	outputTaskDeadline: string;
	outputTaskCreationDate: string;
	outputTaskWeight: number;
	outputTaskGrade: number;
	outputTaskProjectName: string;
	outputTaskProjectID: number;
	outputProjectCategoryName: string;
	outputProjectCategoryID: number;
	outputTaskCustomization: string;
	outputTaskCompletionStatus: number;
}

//these parameter names MUST match up (case-insensitive) with the constructor for TaskRecordInputModel(string inputTaskName, string inputTaskDetails, string inputTaskDeadline, string inputTaskCustomization, string inputTaskWeight, string inputTaskGrade, string inputTaskProjectID, string inputProjectCategoryID, string inputTaskCompletionStatus)
export interface TaskRecordInputModel {
	InputTaskName: string;
	InputTaskDetails: string;
	InputTaskDeadline: string;
	InputTaskCustomization: string;
	InputTaskGrade: string;
	InputTaskWeight: string;
	InputTaskProjectName: string;
	InputTaskProjectID: string;
	InputProjectCategoryName: string;
	InputProjectCategoryID: string;
	InputTaskCompletionStatus: string;
}

export interface TimeSyncModel {
	clientOffset: number;
	serverOffset: number;
}


//https://fireflysemantics.medium.com/using-enum-values-in-angular-templates-abff7df6b4d2
export enum TaskCompletionStatus{
	NoChange = 100,
	NotStarted = 101,
	InProgress = 102,
	Completed = 103,
	Skipped = 104
}

/*
todo: experiment with unknown type
*/
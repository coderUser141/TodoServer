namespace TodoList.Server
{
	public class TaskRecord
	{
        private readonly int _ID;
		private readonly string _taskName;
		private readonly string _taskDetails;
		private readonly DateTime _taskDeadline;
		private readonly DateTime _taskCreationDate;

        public int ID => _ID;
		public string TaskName => _taskName;
		public string TaskDetails => _taskDetails;
		public DateTime TaskDeadline => _taskDeadline;
		public DateTime TaskCreationDate => _taskCreationDate;

		public TaskRecord(int id, string taskname, string taskdetails, DateTime taskdeadline, DateTime? taskcreationdate)
		{
            this._ID = id;
			this._taskName = taskname;
			this._taskDetails = taskdetails;
			this._taskDeadline = taskdeadline;
			this._taskCreationDate = taskcreationdate ?? DateTime.Now;
		}
	}

	public class TaskRecordInputModel
	{
        private readonly string _taskName;
        private readonly string _taskDetails;
        private readonly string _taskDeadline;

        public string TaskName => _taskName;
        public string TaskDetails => _taskDetails;
        public string TaskDeadline => _taskDeadline;

        public TaskRecordInputModel(string taskname, string taskdetails, string taskdeadline)
        {
            this._taskName = taskname;
            this._taskDetails = taskdetails;
            this._taskDeadline = taskdeadline;
        }

	}

    public class TaskRecordOutputModel
    {
        private readonly int _ID;
        private readonly string _taskName;
        private readonly string _taskDetails;
        private readonly string _taskDeadline;
        private readonly string _taskCreationDate;

        public int ID => _ID;
        public string TaskName => _taskName;
        public string TaskDetails => _taskDetails;
        public string TaskDeadline => _taskDeadline;
        public string TaskCreationDate => _taskCreationDate;

        public TaskRecordOutputModel(TaskRecord model)
        {
            this._ID = model.ID;
            this._taskName = model.TaskName;
            this._taskDetails = model.TaskDetails;
            this._taskDeadline = model.TaskDeadline.ToString("s");
            this._taskCreationDate = model.TaskCreationDate.ToString("s");
        }

    }
}

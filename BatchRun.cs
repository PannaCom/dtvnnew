using System;
using youknow.Properties;

namespace youknow
{
    [Serializable]
    public class BatchRun
    {
        #region Constructors

        public BatchRun()
        {
        }

        public BatchRun(int totalNumberOfItems)
        {
            TotalNumberOfItems = totalNumberOfItems;
        }

        #endregion

        #region Properties

        public DateTime? LastUpdatedTime { get; set; }
        public DateTime? StartTime { get; set; }
        public int TotalNumberOfItems { get; set; }
        public int ItemsCompleted { get; private set; }
        public bool ShouldStop { get; set; }
        public string info="";
        public string getInfo{
            get {
                return info;
            }
            set {
                info = value;
            }
        }
        public bool HasNotBegun
        {
            get
            {
                return StartTime == null;
            }
        }

        public bool IsCompletedOrExpired
        {
            get
            {
                return PercentDone == 100 || ShouldStop || (LastUpdatedTime != null && LastUpdatedTime.Value < DateTime.Now.AddMinutes(-Settings.Default.StalledMinuteWait));
            }
        }

        public int PercentDone
        {
            get
            {
                if (TotalNumberOfItems == 0)
                    return 0;
                return (int)(100 * ((double)ItemsCompleted / (double)TotalNumberOfItems));
            }
        }

        private TimeSpan? TotalTime
        {
            get
            {
                if (StartTime == null)
                    return null;
                return DateTime.Now - StartTime.Value;
            }
        }

        public TimeSpan EstimatedTimeRemaining
        {
            get
            {
                if (ItemsCompleted == 0 || TotalTime == null)
                    return default(TimeSpan);
                return TimeSpan.FromSeconds(
                (int)((TotalTime.Value.TotalSeconds / ItemsCompleted) * (TotalNumberOfItems - ItemsCompleted)));
            }
        }
        #endregion

        #region Public Methods
        public void Start()
        {
            if (TotalNumberOfItems == 0)
            {
                throw new ArgumentException("Total Number of Items not set!", "TotalNumberOfItems");
            }
            StartTime = DateTime.Now;
            LastUpdatedTime = DateTime.Now;
        }

        public bool IncrementItemsCompleted()
        {
            if (ItemsCompleted < TotalNumberOfItems)
            {
                LastUpdatedTime = DateTime.Now;
                return (++ItemsCompleted == TotalNumberOfItems);
            }
            return true;
        }
        #endregion
    }
}

using System;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using EpiLastic.Indexing.ReIndexJob;

namespace EpiLastic.Inexing.ReIndexJob
{
    [ScheduledPlugIn(DisplayName = "ReIndexJob")]
    public class ReIndexJob : ScheduledJobBase
    {
        private bool _stopSignaled;

        public ReIndexJob()
        {
            IsStoppable = true;
        }

        /// <summary>
        /// Called when a user clicks on Stop for a manually started job, or when ASP.NET shuts down.
        /// </summary>
        public override void Stop()
        {
            _stopSignaled = true;
        }

        /// <summary>
        /// Called when a scheduled job executes
        /// </summary>
        /// <returns>A status message to be stored in the database log and visible from admin mode</returns>
        public override string Execute()
        {
            var reIndexJobHandler = ServiceLocator.Current.GetInstance<IReIndexJobHandler>();

            //Call OnStatusChanged to periodically notify progress of job for manually started jobs
            OnStatusChanged(String.Format("Starting execution of {0}", this.GetType()));

            var indexedPages = reIndexJobHandler.ReIndex();

            //For long running jobs periodically check if stop is signaled and if so stop execution
            if (_stopSignaled)
            {
                return "Stop of job was called";
            }

            return "Success! Indexed pages: " + indexedPages + " for all languages";
        }
    }
}

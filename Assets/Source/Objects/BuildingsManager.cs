using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Source.Objects
{
    /// <summary>
    /// Controls buildings construction and repairing queue
    /// </summary>
    public class BuildingsManager : MonoBehaviour
    {
        [SerializeField] private int maxWorkersOnOneJob = 3;
        private static BuildingsManager instance;
        public static BuildingsManager Instance => instance ?? (instance = FindObjectOfType<BuildingsManager>());

        private readonly Queue<Job> jobs = new Queue<Job>();

        /// <summary>
        /// Adds object to jobs
        /// </summary>
        /// <param name="buildable"></param>
        public void Add(IBuildable buildable)
        {
            foreach (Job job in jobs)
            {
                if (job.Buildable == buildable)
                {
                    if (job.IsAvailable)
                        return;
                    job.IsAvailable = true;
                    job.WorkersCount = 0;
                    return;
                }
            }
            jobs.Enqueue(new Job(buildable));
        }

        /// <summary>
        /// Removes object from jobs
        /// </summary>
        /// <param name="buildable"></param>
        public void Remove(IBuildable buildable)
        {
            Job job = jobs.FirstOrDefault(j => j.Buildable == buildable);
            if (job != null)
                job.IsAvailable = false;
        }

        /// <summary>
        /// Free job from a worker
        /// </summary>
        /// <param name="buildable"></param>
        public void Free(IBuildable buildable)
        {
            Job job = jobs.FirstOrDefault(j => j.Buildable == buildable);
            if (job != null)
                job.WorkersCount --;
        }

        /// <summary>
        /// Get job if possible
        /// </summary>
        /// <returns>Job or null if non</returns>
        public IBuildable GetJob()
        {
            if (!jobs.Any())
                return null;

            for (int i = 0; i < maxWorkersOnOneJob; i++)
            {
                Job job = null;

                for (int j = 0; j < jobs.Count; j++)
                {
                    job = jobs.Dequeue();
                    if (!job.IsAvailable)
                        continue;
                    jobs.Enqueue(job);
                    if (job.WorkersCount == i)
                        break;
                }

                if (!jobs.Any() || job == null)
                    return null;

                if (job.WorkersCount != i)
                    continue;

                job.WorkersCount++;
                return job.Buildable;
            }

            return null;
        }

        private class Job
        {
            public readonly IBuildable Buildable;

            public int WorkersCount = 0;

            public bool IsAvailable = true;

            public Job(IBuildable buildable)
            {
                Buildable = buildable;
            }
        }
    }
}

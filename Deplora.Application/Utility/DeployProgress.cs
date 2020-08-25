using Deplora.Shared.Enums;
using System;
using System.Linq;

namespace Deplora.App.Utility
{
    public class DeployProgress
    {
        public DeployProgress(DeployStep step, string message)
        {
            this.DeployStep = step;
            this.Message = message;
        }

        public DeployStep DeployStep { get; set; }
        public decimal ProgressPercentage { get => GetProgressBasedOnStep(this.DeployStep) * 100; }
        private string message;
        public string Message { get => this.message; set => this.message = string.Format("{0}: {1}", DateTime.Now, value); }

        private decimal GetProgressBasedOnStep(DeployStep step)
        {
            var maxStep = ((DeployStep[])Enum.GetValues(typeof(DeployStep))).Where(ds => ds != DeployStep.Rollback).LastOrDefault();
            return (decimal)step / (maxStep != 0 ? (decimal)maxStep : throw new InvalidOperationException("Enum has no values!"));
        }
    }
}

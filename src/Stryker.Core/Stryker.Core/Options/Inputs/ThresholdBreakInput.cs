﻿using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    // This does not work because of the helptext
    public class ThresholdBreakInput : ComplexStrykerInput<string, int>
    {
        public override StrykerInput Type => StrykerInput.ThresholdBreak;
        public override string DefaultInput => DefaultValue.ToString();
        public override int DefaultValue => 0;

        protected override string Description => "Anything below this mutation score will return a non-zero exit code. Must be less than threshold low.";
        protected override string HelpOptions => FormatHelpOptions("0 - 99");

        public ThresholdBreakInput() { }
        public ThresholdBreakInput(string inputBreak, int low)
        {
            if (inputBreak is { })
            {
                var @break = int.Parse(inputBreak);
                if (@break > 99 || @break < 0)
                {
                    throw new StrykerInputException("Threshold break must be between 0 and 99.");
                }

                if (@break <= low)
                {
                    throw new StrykerInputException($"Threshold break must be less than threshold high. Current low: {low}, break: {@break}");
                }

                Value = @break;
            }
        }
    }
}

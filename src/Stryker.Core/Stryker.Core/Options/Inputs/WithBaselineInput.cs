namespace Stryker.Core.Options.Inputs
{
    public class WithBaselineInput : InputDefinition<bool?>
    {
        public override bool? Default => false;

        protected override string Description => "EXPERIMENTAL: Use results stored in stryker dashboard to only test new mutants.";

        public WithBaselineInput() { }

        public bool Validate()
        {
            return SuppliedInput.IsNotNullAndTrue();
        }
    }
}

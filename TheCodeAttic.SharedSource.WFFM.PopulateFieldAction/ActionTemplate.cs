using Sitecore.Forms.Core.Rules;

namespace TheCodeAttic.SharedSource.WFFM.PopulateFieldAction
{
    public class ActionTemplate<T> : ReadValue<T> where T : ConditionalRuleContext
    {
        protected override object GetValue()
        {
            string retVal = string.Empty;
            /// This value is inherited from ReadValue<T>.
            /// This is the will contain the selected/entered value to be acted on by this action
            /// In some instances you may not need to use this for your action
            var ValueToPeformActionLogicOn = this.Name;
            
            ///Custom action logic goes here

            return (object)retVal; //return value to be assigned to the form field.
        }
    }
}
namespace DragynGames.Editor.ScriptGeneration
{
    public class TemplateConverter : IConvertTemplate
    {
        public string Convert(string templateContent, string scriptName)
        {
            return templateContent.Replace("#SCRIPTNAME#", scriptName);
        }
    }
}
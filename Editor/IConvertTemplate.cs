namespace DragynGames.Editor.ScriptGeneration
{
    public interface IConvertTemplate
    {
        string Convert(string templateContent, string scriptName);
    }
}
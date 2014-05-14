public interface ICoreInspectable
{
    bool OnInspectorGUI();
}
public interface ICoreInspectableWithDefaultView : ICoreInspectable
{
}
public interface ICoreInspectableWithFoldedDefaultView : ICoreInspectable
{
}
public interface ICoreInspectableWithoutDefaultView : ICoreInspectable
{
}
	
public interface ICoreSceneDrawable
{
	void OnSceneGUI();
}
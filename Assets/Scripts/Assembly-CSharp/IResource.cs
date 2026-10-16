public interface IResource
{
	IResourceGroup resourceGroup { get; set; }

	string identifier { get; set; }

	object ResourceObject { get; set; }

	void Unload();
}

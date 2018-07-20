using ZXMAK2.Host.Entities;


namespace ZXMAK2.Host.Interfaces
{
	public interface IKeyboardState
	{
		bool this[Key key] { get; }
	}
}

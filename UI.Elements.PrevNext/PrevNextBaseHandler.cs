using Core.Handler;

namespace UI.Elements.PrevNext;

public abstract class PrevNextBaseHandler : BaseHandler
{
	protected string Value;

	public void Setup(string value)
	{
		Value = value;
	}
}

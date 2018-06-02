using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitScreen.Menu
{
	class DetachKeyboardButton : BaseTextButton
	{
		Keyboards.MultipleKeyboardManager keyboardManager;

		public DetachKeyboardButton(int x, int y, Keyboards.MultipleKeyboardManager keyboardManager) : base(x, y, "Detach keyboard")
		{
			this.keyboardManager = keyboardManager;

			base.isDisabled = ModEntry._playerIndexController._PlayerIndex != null;
		}

		public override void OnClicked()
		{
			keyboardManager.OnDetachButtonClicked();
		}
	}
}

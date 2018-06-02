using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitScreen.Menu
{
	class DetachMouseButton : BaseTextButton
	{
		Mice.MultipleMiceManager miceManager;

		public DetachMouseButton(int x, int y, Mice.MultipleMiceManager miceManager) : base(x, y, "Detach mouse")
		{
			this.miceManager = miceManager;

			base.isDisabled = ModEntry._playerIndexController._PlayerIndex != null;
		}

		public override void OnClicked()
		{
			miceManager.DetachMouseButtonClicked();
		}
	}
}

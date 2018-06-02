using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace SplitScreen.Menu
{
	class AttachMouseButton : BaseTextButton
	{
		Mice.MultipleMiceManager miceManager;

		public AttachMouseButton(int x, int y, Mice.MultipleMiceManager miceManager) : base(x,y, "Attach mouse")
		{
			this.miceManager = miceManager;

			base.isDisabled = ModEntry._playerIndexController._PlayerIndex != null;
		}

		public override void OnClicked()
		{
			Monitor.Log("Attach button clicked", LogLevel.Trace);
			miceManager.AttachMouseButtonClicked();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ProjectReferenceContextMenuExtension.Commands
{
	internal class CommandBase
	{
		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		protected readonly Package package;

		protected CommandBase(Package package)
		{
			if (package == null)
			{
				throw new ArgumentNullException(nameof(package));
			}

			this.package = package;
		}

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		protected IServiceProvider ServiceProvider
		{
			get
			{
				return package;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="urls"></param>
		/// <param name="title"></param>
		/// <param name="message"></param>
		/// <param name="delay"></param>
		protected void LaunchInBrowser(List<string> urls, string title, string message, int delay = 0)
		{
			if (urls.Count == 0)
			{
				// show an info message
				VsShellUtilities.ShowMessageBox(
					ServiceProvider,
					message,
					title,
					OLEMSGICON.OLEMSGICON_INFO,
					OLEMSGBUTTON.OLEMSGBUTTON_OK,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
			}
			else if (delay == 0)
			{
				// TODO: allow user to specify default browser to use
				urls.ForEach(x => System.Diagnostics.Process.Start(x));
			}
			else {
				// TODO: allow user to specify default browser to use
				urls.Distinct().ToList().ForEach(x => {
					System.Diagnostics.Process.Start(x);
					// need a delay s StackOverflow doesn't think we're a bot
					System.Threading.Thread.Sleep(delay);
				});
			}
		}
	}
}
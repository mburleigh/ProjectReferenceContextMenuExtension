using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;

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
	}
}
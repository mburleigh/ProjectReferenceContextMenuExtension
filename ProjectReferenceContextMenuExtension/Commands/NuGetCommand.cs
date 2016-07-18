//------------------------------------------------------------------------------
// <copyright file="NuGetCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

using Microsoft.VisualStudio.Shell;

using EnvDTE80;
using EnvDTE;

namespace ProjectReferenceContextMenuExtension.Commands
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class NuGetCommand : CommandBase
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 256;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("8fdac97d-17b5-46ea-b9d4-8d12d246d495");

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        NuGetCommand(Package package) : base(package)
        {
            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

		/// <summary>
		/// Gets the instance of the command.
		/// </summary>
		public static NuGetCommand Instance
		{
			get;
			private set;
		}

		/// <summary>
		/// Initializes the singleton instance of the command.
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		public static void Initialize(Package package)
        {
            Instance = new NuGetCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        void MenuItemCallback(object sender, EventArgs e)
        {
            var dte = (DTE2)ServiceProvider.GetService(typeof(DTE));
            var nugets = _CommonCommandUtilities.ProcessSelectedNuGetPackages<List<string>>(dte,
				(collection, path) => {
					var packageId = _CommonCommandUtilities.GetValueFromNupkg(path, "id");
					if (!string.IsNullOrEmpty(packageId))
						collection.Add($"https://www.nuget.org/packages/{packageId}");
				});

			// TODO: allow user to specify default browser to use
			nugets.Distinct().ToList().ForEach(x => System.Diagnostics.Process.Start(x));
        }
	}
}
//------------------------------------------------------------------------------
// <copyright file="VersionCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using EnvDTE80;
using EnvDTE;

namespace ProjectReferenceContextMenuExtension.Commands
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class VersionCommand : CommandBase
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 0x0100;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("866c9883-9b23-4d2a-a8d1-c26984ef526b");

		/// <summary>
		/// Initializes a new instance of the <see cref="VersionCommand"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		VersionCommand(Package package) : base(package)
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
		public static VersionCommand Instance
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
			Instance = new VersionCommand(package);
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
			//var message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", GetType().FullName);

			var dte = (DTE2)ServiceProvider.GetService(typeof(DTE));
			var versions = GetSelectedReferencesVersions(dte);

			//var message = string.Join(Environment.NewLine, );

			var sb = new System.Text.StringBuilder();
			foreach (var item in versions)
			{
				sb.AppendLine($"{item.Key}: {item.Value}");
			}


			string title = "Package Version(s):";

			// Show a message box to prove we were here
			VsShellUtilities.ShowMessageBox(
				ServiceProvider,
				sb.ToString(),
				title,
				OLEMSGICON.OLEMSGICON_INFO,
				OLEMSGBUTTON.OLEMSGBUTTON_OK,
				OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
		}

        //static IEnumerable<string> GetSelected(DTE2 dte)
        //{
        //	var selectedItems = (Array)dte.ToolWindows.SolutionExplorer.SelectedItems;

        //	var hierarchyObjects = selectedItems.Cast<UIHierarchyItem>().Select(x => x.Object).ToList();

        //	//return new List<string> { ((dynamic)xxx.First()).Path };

        //	//return xxx.Select(x => ((dynamic)x).Path).ToList();

        //	var result = new List<string>();
        //	foreach (var item in hierarchyObjects)
        //	{
        //		result.Add(((dynamic)item).Path);
        //	}
        //	return result;
        //}

        static Dictionary<string, string> GetSelectedReferencesVersions(DTE2 dte)
        {
            var result = new Dictionary<string, string>();
            foreach (var item in _CommonCommandUtilities.GetSelectedReferences(dte))
            {
                var d = (dynamic)item;
                result.Add(d.Name, d.Version);
            }
            return result;
        }
    }
}
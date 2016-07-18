using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

using EnvDTE;
using EnvDTE80;

namespace ProjectReferenceContextMenuExtension.Commands
{
    class _CommonCommandUtilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dte"></param>
        /// <returns></returns>
        internal static IEnumerable<object> GetSelectedReferences(DTE2 dte)
        {
            // TODO: restrict to project references

            var selectedItems = (Array)dte.ToolWindows.SolutionExplorer.SelectedItems;
            return selectedItems.Cast<UIHierarchyItem>().Select(x => x.Object);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dllPath"></param>
		/// <param name="valueName"></param>
		/// <returns></returns>
		internal static string GetValueFromNupkg(string dllPath, string valueName)
		{
			var nupkg = FindNupkg(dllPath);

			if (!string.IsNullOrEmpty(nupkg))
			{
				// unzip nupkg file
				using (ZipArchive package = ZipFile.OpenRead(nupkg))
				{
					foreach (ZipArchiveEntry entry in package.Entries)
					{
						if (entry.FullName.EndsWith("nuspec", StringComparison.OrdinalIgnoreCase))
						{
							using (var nuspecStream = entry.Open())
							{
								// dig out the specified value
								var nuspec = XDocument.Load(nuspecStream);

								var element = nuspec.Descendants().FirstOrDefault(x => x.Name.LocalName.Equals(valueName));
								if (element != null)
									return element.Value;
							}
							break;
						}
					}
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dllPath"></param>
		/// <returns></returns>
		static string FindNupkg(string dllPath)
		{
			// walk up from the DLL to find the .nupkg
			while (dllPath.Length > 0)
			{
				if (Directory.Exists(dllPath))
				{
					foreach (var file in Directory.GetFiles(dllPath))
					{
						if (file.EndsWith("nupkg", StringComparison.OrdinalIgnoreCase))
							return file;
					}
				}

				// chop off the last element
				dllPath = dllPath.Substring(0, dllPath.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase));
			}

			return string.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dte"></param>
		/// <param name="selectedProcessor"></param>
		/// <returns></returns>
		internal static T ProcessSelectedNuGetPackages<T>(DTE2 dte, Action<T, string> selectedProcessor) 
			where T : new()
		{
			var result = new T();
			foreach (var item in GetSelectedReferences(dte))
			{
				var d = (dynamic)item;

				// restrict to NuGet packages
				if (d.Path.ToLower().Contains("packages"))
				{
					selectedProcessor(result, d.Path);
				}
			}
			return result;
		}
	}
}
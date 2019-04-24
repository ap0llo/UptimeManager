// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System;
using System.Xml.Linq;

namespace UptimeManager.Configuration.Xml
{
    /// <summary>
    /// Extension methods for XElement
    /// </summary>
    static class XElementExtensions
    {
        /// <summary>
        /// Returns the value if the specifed attribute. If the attribute is not present a ConfigurationException will be
        /// thrown
        /// </summary>
        public static string RequireAttributeValue(this XElement element, string attributeName)
        {
            var attribute = element.Attribute(attributeName);

            if (attribute == null)
            {
                throw new ConfigurationException(String.Format("Attribute '{0}' not present in element '{1}'",
                    attributeName, element.Name.LocalName));
            }
            return attribute.Value;
        }

        /// <summary>
        /// Changes the namespace of every element in the currentNamespace in the element tree to newNamespace
        /// </summary>
        public static void ReplaceNamespace(this XElement element, XNamespace currentNamespace, XNamespace newNamespace)
        {
            if (element.Name.Namespace == currentNamespace)
            {
                element.Name = newNamespace.GetName(element.Name.LocalName);
            }

            foreach (var childNode in element.Elements())
            {
                childNode.ReplaceNamespace(currentNamespace, newNamespace);
            }
        }
    }
}
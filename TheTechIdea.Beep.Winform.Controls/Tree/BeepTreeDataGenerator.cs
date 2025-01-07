using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Desktop.Common;

namespace TheTechIdea.Beep.Winform.Controls.Tree
{
    public static class BeepTreeDataGenerator
    {
        /// <summary>
        /// Generates a BindingList of SimpleItem objects representing a hierarchical tree structure.
        /// </summary>
        /// <param name="numRootNodes">Number of root nodes to generate.</param>
        /// <param name="numLevels">Number of levels each root node will have.</param>
        /// <param name="numNodesPerLevel">Number of nodes at each level.</param>
        /// <returns>A BindingList of SimpleItem objects populated with the specified hierarchy.</returns>
        public static BindingList<SimpleItem> GenerateMockData(int numRootNodes, int numLevels, int numNodesPerLevel)
        {
            if (numRootNodes <= 0)
                throw new ArgumentException("Number of root nodes must be greater than zero.", nameof(numRootNodes));
            if (numLevels <= 0)
                throw new ArgumentException("Number of levels must be greater than zero.", nameof(numLevels));
            if (numNodesPerLevel <= 0)
                throw new ArgumentException("Number of nodes per level must be greater than zero.", nameof(numNodesPerLevel));

            BindingList<SimpleItem> rootItems = new BindingList<SimpleItem>();

            for (int rootIndex = 1; rootIndex <= numRootNodes; rootIndex++)
            {
                SimpleItem rootItem = new SimpleItem
                {
                    Text = $"Root Node {rootIndex}",
                    ImagePath = "path/to/root/image.png", // Replace with actual image paths if needed
                    MenuID = $"RootMenu{rootIndex}"
                };

                // Recursively generate child nodes
                GenerateChildNodes(rootItem, currentLevel: 1, maxLevels: numLevels, nodesPerLevel: numNodesPerLevel, parentName: $"Root{rootIndex}");

                rootItems.Add(rootItem);
            }

            return rootItems;
        }

        /// <summary>
        /// Recursively generates child nodes for a given parent node.
        /// </summary>
        /// <param name="parent">The parent SimpleItem to which child nodes will be added.</param>
        /// <param name="currentLevel">The current depth level in the tree.</param>
        /// <param name="maxLevels">The maximum depth level to generate.</param>
        /// <param name="nodesPerLevel">The number of child nodes to generate at each level.</param>
        /// <param name="parentName">A string representing the parent's name for unique identification.</param>
        private static void GenerateChildNodes(SimpleItem parent, int currentLevel, int maxLevels, int nodesPerLevel, string parentName)
        {
            if (currentLevel >= maxLevels)
                return; // Base case: reached the maximum level

            for (int i = 1; i <= nodesPerLevel; i++)
            {
                string nodeName = $"{parentName}_Level{currentLevel}_Node{i}";
                SimpleItem childItem = new SimpleItem
                {
                    Text = $"Node {i} at Level {currentLevel} (Parent: {parent.Text})",
                    ImagePath = "path/to/child/image.png", // Replace with actual image paths if needed
                    MenuID = $"Menu_{nodeName}",
                    ParentItem = parent

                };

                parent.Children.Add(childItem);

                // Recursive call to generate the next level of child nodes
                GenerateChildNodes(childItem, currentLevel + 1, maxLevels, nodesPerLevel, nodeName);
            }
        }
    }
}

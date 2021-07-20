﻿using System;
using Smartstore.Engine.Modularity;

namespace Smartstore.Core.Packaging
{
    internal class BareExtensionDescriptor : IExtensionDescriptor
    {
        public BareExtensionDescriptor()
        {
        }

        public BareExtensionDescriptor(IExtensionDescriptor copyFrom)
        {
            ExtensionType = copyFrom.ExtensionType;
            Name = copyFrom.Name;
            FriendlyName = copyFrom.FriendlyName;
            Description = copyFrom.Description;
            Group = copyFrom.Group;
            Author = copyFrom.Author;
            ProjectUrl = copyFrom.ProjectUrl;
            Tags = copyFrom.Tags;
            Version = copyFrom.Version;
            MinAppVersion = copyFrom.MinAppVersion;
        }

        /// <inheritdoc/>
        public ExtensionType ExtensionType { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string FriendlyName { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public string Group { get; set; }

        /// <inheritdoc/>
        public string Author { get; set; }

        /// <inheritdoc/>
        public string ProjectUrl { get; set; }

        /// <inheritdoc/>
        public string Tags { get; set; }

        /// <inheritdoc/>
        public SemanticVersion Version { get; set; }

        /// <inheritdoc/>
        public SemanticVersion MinAppVersion { get; set; }
    }
}
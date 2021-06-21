﻿using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smartstore.ComponentModel;
using Smartstore.Core.Content.Media;
using Smartstore.Web.Modelling;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Smartstore.Admin.Models
{
    [LocalizedDisplay("Admin.Configuration.Settings.Media.")]
    public class MediaSettingsModel : ModelBase
    {
        [LocalizedDisplay("*AutoGenerateAbsoluteUrls")]
        public bool AutoGenerateAbsoluteUrls { get; set; }

        [LocalizedDisplay("*MaximumImageSize")]
        public int MaximumImageSize { get; set; }

        [LocalizedDisplay("*MaxUploadFileSize")]
        public long MaxUploadFileSize { get; set; }

        [LocalizedDisplay("*MakeFilesTransientWhenOrphaned")]
        public bool MakeFilesTransientWhenOrphaned { get; set; }

        [LocalizedDisplay("*DefaultPictureZoomEnabled")]
        public bool DefaultPictureZoomEnabled { get; set; }

        [LocalizedDisplay("*StorageProvider")]
        public string StorageProvider { get; set; }
        
        #region Thumbnail sizes

        [LocalizedDisplay("*AvatarPictureSize")]
        public int AvatarPictureSize { get; set; }

        [LocalizedDisplay("*ProductThumbPictureSize")]
        public int ProductThumbPictureSize { get; set; }

        [LocalizedDisplay("*ProductDetailsPictureSize")]
        public int ProductDetailsPictureSize { get; set; }

        [LocalizedDisplay("*ProductThumbPictureSizeOnProductDetailsPage")]
        public int ProductThumbPictureSizeOnProductDetailsPage { get; set; }

        [LocalizedDisplay("*MessageProductThumbPictureSize")]
        public int MessageProductThumbPictureSize { get; set; }

        [LocalizedDisplay("*AssociatedProductPictureSize")]
        public int AssociatedProductPictureSize { get; set; }

        [LocalizedDisplay("*BundledProductPictureSize")]
        public int BundledProductPictureSize { get; set; }

        [LocalizedDisplay("*CategoryThumbPictureSize")]
        public int CategoryThumbPictureSize { get; set; }

        [LocalizedDisplay("*ManufacturerThumbPictureSize")]
        public int ManufacturerThumbPictureSize { get; set; }

        [LocalizedDisplay("*CartThumbPictureSize")]
        public int CartThumbPictureSize { get; set; }

        [LocalizedDisplay("*CartThumbBundleItemPictureSize")]
        public int CartThumbBundleItemPictureSize { get; set; }

        [LocalizedDisplay("*MiniCartThumbPictureSize")]
        public int MiniCartThumbPictureSize { get; set; }

        public int[] CurrentlyAllowedThumbnailSizes { get; set; }

        #endregion

        #region Media types

        [UIHint("Textarea")]
        [AdditionalMetadata("rows", 2)]
        [LocalizedDisplay("*Type.Image")]
        public string ImageTypes { get; set; }

        [UIHint("Textarea")]
        [AdditionalMetadata("rows", 2)]
        [LocalizedDisplay("*Type.Video")]
        public string VideoTypes { get; set; }

        [UIHint("Textarea")]
        [AdditionalMetadata("rows", 2)]
        [LocalizedDisplay("*Type.Audio")]
        public string AudioTypes { get; set; }

        [UIHint("Textarea")]
        [AdditionalMetadata("rows", 2)]
        [LocalizedDisplay("*Type.Document")]
        public string DocumentTypes { get; set; }

        [UIHint("Textarea")]
        [AdditionalMetadata("rows", 2)]
        [LocalizedDisplay("*Type.Text")]
        public string TextTypes { get; set; }

        [UIHint("Textarea")]
        [AdditionalMetadata("rows", 2)]
        [LocalizedDisplay("*Type.Bin")]
        public string BinTypes { get; set; }

        #endregion
    }

    public partial class MediaSettingsValidator : AbstractValidator<MediaSettingsModel>
    {
        public MediaSettingsValidator()
        {
            RuleFor(x => x.MaxUploadFileSize).GreaterThan(0);
        }
    }

    public class MediaSettingsMapper : IMapper<MediaSettings, MediaSettingsModel>, IMapper<MediaSettingsModel, MediaSettings>
    {
        public Task MapAsync(MediaSettings from, MediaSettingsModel to, dynamic parameters = null)
        {
            MiniMapper.Map(from, to);

            to.ImageTypes = MapMediaType(from.ImageTypes, MediaType.Image);
            to.VideoTypes = MapMediaType(from.VideoTypes, MediaType.Video);
            to.AudioTypes = MapMediaType(from.AudioTypes, MediaType.Audio);
            to.DocumentTypes = MapMediaType(from.DocumentTypes, MediaType.Document);
            to.TextTypes = MapMediaType(from.TextTypes, MediaType.Text);
            to.BinTypes = MapMediaType(from.BinTypes, MediaType.Binary);

            return Task.CompletedTask;
        }

        public Task MapAsync(MediaSettingsModel from, MediaSettings to, dynamic parameters = null)
        {
            MiniMapper.Map(from, to);

            to.ImageTypes = MapMediaType(from.ImageTypes, MediaType.Image);
            to.VideoTypes = MapMediaType(from.VideoTypes, MediaType.Video);
            to.AudioTypes = MapMediaType(from.AudioTypes, MediaType.Audio);
            to.DocumentTypes = MapMediaType(from.DocumentTypes, MediaType.Document);
            to.TextTypes = MapMediaType(from.TextTypes, MediaType.Text);
            to.BinTypes = MapMediaType(from.BinTypes, MediaType.Binary);

            return Task.CompletedTask;
        }

        private static string MapMediaType(string types, MediaType mediaType)
        {
            return types.NullEmpty() ?? string.Join(" ", mediaType.DefaultExtensions);
        }
    }
}
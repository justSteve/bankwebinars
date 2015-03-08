using CUWebinars.Business.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CUWebinars.Web.ViewModel
{
    public class WebinarEditModel
    {
        public int idWebinar { get; set; }
        public int idPresenter { get; set; }
        public string Description { get; set; }
        public string DescriptionLong { get; set; }
        public string ImageUrl { get; set; }
        public string SmallImageUrl { get; set; }
        public IEnumerable<SelectListItem> Statuses { get; set; }
        public string RecordingUrl { get; set; }
        public string Title { get; set; }
        public System.DateTime Date { get; set; }
        public string LearnCaption { get; set; }
        public string LearnBody { get; set; }
        public string WhoAttend { get; set; }
        public decimal Duration { get; set; }
        public string ceu { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateChanged { get; set; }
        public IEnumerable<SelectListItem> Presenters { get; set; }
        public decimal AdditionalLocationsPrice { get; set; }
        public int SelectedPresenter { get; set; }
        public int SelectedStatus { get; set; }
        public PostedTopics PostedTopics { get; set; }
        public PostedRegTypeGroups PostedRegTypeGroups { get; set; }
        public IEnumerable<Topic> Topics { get; set; }
        public IEnumerable<Topic> SelectedTopics { get; set; }
        public IEnumerable<RegTypesGroup> RegTypeGroups { get; set; }
        public IEnumerable<RegTypesGroup> SelectedRegTypeGroups { get; set; }
    }

    /// <summary>
    /// Helper class in support of the CheckBoxList. Makes it easier for ModelBinder to deserialize.
    /// </summary>
    public class PostedTopics
    {
        public int[] TopicIds { get; set; }
    }
    
    /// <summary>
    /// Helper class in support of the CheckBoxList. Makes it easier for ModelBinder to deserialize.
    /// </summary>
    public class PostedRegTypeGroups
    {
        public int[] RegTypeGroupIds { get; set; }
    }
}
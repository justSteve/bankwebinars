using System.Data.Entity;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.NotificationSystem.Event;
using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using FluentValidation;
using Ninject.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using CUWebinars.Business.ModelsV4;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IEvent = CUWebinars.NotificationSystem.Event.IEvent;

namespace CUWebinars.Business.Services
{
    public class WebinarManagementService : IEventSource, IWebinarManagementService
    {
        //private readonly IAffiliateRepository _affiliateRepository;
        private readonly IRegTypeRepository _regTypeRepository;
        //private readonly IOrderRepository _orderRepository;
        private readonly IRefDataRepository _refDataRepository;
        private readonly IWebinarRepository _webinarRepository;
        private readonly IWebinarFileRepository _webinarFileRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly ILogger _logger;
        private readonly IWebUserRepository _webUserRepository;
        private readonly TtsConfiguration _ttsConfig;
        private readonly IValidator<Webinar> _createWebinarValidator;
        private readonly IValidator<Webinar> _updateWebinarValidator;
        readonly List<IEvent> _events = new List<IEvent>();
        private bool _disposed;

        public WebinarManagementService(
            //IAffiliateRepository affiliateRepository,
            IRegTypeRepository regTypeRepository,
            //IOrderRepository orderRepository,
            IRefDataRepository refDataRepository,
            IWebUserRepository webUserRepository,
            IWebinarRepository webinarRepository,
            IWebinarFileRepository webinarFileRepository,
            IQuizRepository quizRepository,
            ILogger logger,
            TtsConfiguration ttsConfig,
            IValidator<Webinar> createWebinarValidator,
            IValidator<Webinar> updateWebinarValidator)
        {
            //_affiliateRepository = affiliateRepository;
            _regTypeRepository = regTypeRepository;
            //_orderRepository = orderRepository;
            _refDataRepository = refDataRepository;
            _ttsConfig = ttsConfig;
            _createWebinarValidator = createWebinarValidator;
            _updateWebinarValidator = updateWebinarValidator;
            _webinarFileRepository = webinarFileRepository;
            _quizRepository = quizRepository;
            _webinarRepository = webinarRepository;
            _logger = logger;
            _webUserRepository = webUserRepository;
        }
        public IEnumerable<Presenter> GetAllPresenters()
        {
            try
            {
                return _refDataRepository.GetAllPresenters();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("GetAllPresenters method", exception);
                throw;
            }

        }

        public IEnumerable<Topic> GetAllTopics()
        {
            return _webinarRepository.GetAllTopics();
        }

        //public void AddAdditionalLocationsLookupPrice(AdditionalLocationsLookupPrice additionalLocationsLookupPrice)
        //{
        //    // NOTE: db.SaveChanges is not called in the following method. 
        //    _webinarRepository.AddAdditionalLocationsLookupPrice(additionalLocationsLookupPrice);
        //}

        public bool AddQuestionsToQuiz(int selectedWebinar, IEnumerable<Question> newQuestions)
        {
            var quiz = _quizRepository.GetQuizByWebinarId(selectedWebinar);

            // if there are any QuizUserAnswers for this quiz, this means someone has taken the quiz and no questions can be deleted.
            if (quiz.QuizWithQuestions.SelectMany(q => q.QuizUserAnswers).Any())
            {
                return false;
            }

            foreach (var question in newQuestions)
            {
                _quizRepository.AddQuestion(question);

                var options = question.QuestionWithOptions.Select(q => q.Option).ToList();

                // This loop is an edge case. true and false will be very common answers. Rather than storing them over and over again,
                // this loop will re-use the already stored answers for true and false, which complies with the quiz schema.

                var optionsToDelete = new List<Option>();

                foreach (var option in options)
                {
                    if (option.Text.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                        option.Text.Equals("false", StringComparison.OrdinalIgnoreCase))
                    {
                        var storedOption = _quizRepository.Context.Option.Single(o => o.Id == option.Id);

                        foreach (var questionWithOption in question.QuestionWithOptions)
                        {
                            if (questionWithOption.Option.Id == storedOption.Id)
                            {
                                optionsToDelete.Add(option);
                                questionWithOption.idOption = storedOption.Id;
                            }
                        }
                    }
                }

                _quizRepository.Context.Option.RemoveRange(optionsToDelete);
                _quizRepository.SaveChanges();
            }

            return true;
        }

        public void AddWebinar(Webinar webinar)
        {
            _createWebinarValidator.ValidateAndThrow(webinar);

            _webinarRepository.Add(webinar);
        }

        public void AddWebinarFiles(IEnumerable<WebinarFile> webinarFiles)
        {
            _webinarFileRepository.AddRange(webinarFiles);
        }

        public void DeleteWebinar(int webinarId)
        {
            var webinar = GetWebinar(webinarId);
            _webinarRepository.Delete(webinar);
        }

        public void DeleteWebinarFiles(IEnumerable<WebinarFile> webinarFiles)
        {
            _webinarFileRepository.DeleteRange(webinarFiles);
        }

        public void DeleteRegTypeGroupXRef(Webinar webinar, int idRegTypeGroupXRef)
        {
            var regTypeGroupXRef = _webinarRepository.GetRegTypesGroupsXref(idRegTypeGroupXRef, webinar.idWebinar);

            webinar.RegTypesGroupsXref.Remove(regTypeGroupXRef);

            _webinarRepository.MarkForDeletion(regTypeGroupXRef);
        }

        public void DeleteWebinarTopicXref(Webinar webinar, int exisingTopicId)
        {
            var webinarTopicXRef = _webinarRepository.GetWebinarTopicXref(exisingTopicId, webinar.idWebinar);

            webinar.WebinarTopicXrefs.Remove(webinarTopicXRef);

            _webinarRepository.MarkForDeletion(webinarTopicXRef);
        }

        public Quiz GetQuizByCode(string quizCode)
        {
            return _quizRepository.GetQuizByCode(quizCode);
        }

        public QuestionCountAndWebinarId GetQuizQuestionCountAndWebinarId(string quizCode)
        {
            return _quizRepository.GetQuestionCountAndWebinarId(quizCode);
        }

        public Quiz GetQuizByQuizId(int quizId)
        {
            return _quizRepository.GetQuizByIdWithOptions(quizId);
        }

        public IList<QuizScore> GetQuizScoreForUser(int quizId, string email, int orderId)
        {
            var quiz = _quizRepository.GetQuizByIdWithOptionsAndUserAnswers(quizId);

            // ReSharper disable once TooWideLocalVariableScope
            bool isCorrect;
            var questionResult = new Dictionary<int, KeyValuePair<char, bool>>();
            IList<QuizScore> quizScores = new List<QuizScore>();

            foreach (var quizUserOrder in quiz.QuizUserOrders.Where(quo => quo.Email == email && quo.idQuiz == quizId && quo.idOrder == orderId))
            {
                int talley = 0;

                QuizUserOrder order = quizUserOrder; // to avoid unpredicted behaviour if different versions of the compiler compile this code.

                questionResult = new Dictionary<int, KeyValuePair<char, bool>>();

                foreach (var quizWithQuestion in quiz.QuizWithQuestions.Where(qwq => qwq.idQuiz == order.idQuiz))
                {
                    foreach (var quizUserAnswer in quizWithQuestion.QuizUserAnswers.Where(qua => qua.idQuizUserOrder == order.Id))
                    {
                        isCorrect = false;

                        if (quizUserAnswer.Letter.Contains(quizUserAnswer.QuizWithQuestion.Question.QuestionWithOptions.Single(qwo => qwo.CorrectAnswer).Letter))
                        {
                            isCorrect = true;
                            talley++;
                        }

                        questionResult.Add(quizWithQuestion.QuestionNumber, new KeyValuePair<char, bool>(quizUserAnswer.Letter.First(), isCorrect));
                    }
                }
                quizScores.Add(new QuizScore { QuestionResult = questionResult, TotalCorrectAnswerCount = talley, TotalQuestionCount = quiz.QuizWithQuestions.Count });
            }

            return quizScores;
        }

        public bool RemoveQuestionsFromQuiz(IEnumerable<int> deletedQuestions, int quizId)
        {
            var quiz = _quizRepository.GetQuizByIdWithOptions(quizId);

            // if there are any QuizUserAnswers for this quiz, this means someone has taken the quiz and no questions can be deleted.
            if (quiz.QuizWithQuestions.SelectMany(q => q.QuizUserAnswers).Any())
            {
                return false;
            }

            foreach (var deletedQuestion in deletedQuestions)
            {
                var quizWithQuestion = quiz.QuizWithQuestions.FirstOrDefault(q => q.Id == deletedQuestion);

                if (!ReferenceEquals(null, quizWithQuestion))
                {
                    _quizRepository.DeleteQuizWithQuestion(quizWithQuestion);
                }
            }
            _quizRepository.SaveChanges();

            return true;
        }

        public bool UpdateQuestions(IEnumerable<EditedQuestion> editedQuestions, int quizId)
        {
            var quiz = _quizRepository.GetQuizByIdWithOptionsText(quizId);

            // if there are any QuizUserAnswers for this quiz, this means someone has taken the quiz and no questions can be deleted.
            if (quiz.QuizWithQuestions.SelectMany(q => q.QuizUserAnswers).Any())
            {
                return false;
            }

            foreach (var editedQuestion in editedQuestions)
            {
                var question = quiz.QuizWithQuestions.Single(q => q.Id == editedQuestion.QuestionId).Question;

                if (editedQuestion.EditedOptions != null)
                {
                    // get deleted options list
                    var deletedOptions = editedQuestion.EditedOptions.Where(e => e.EditType == EditType.Deleted);
                    // get added options list
                    var addedOptions = editedQuestion.EditedOptions.Where(e => e.EditType == EditType.Added);
                    // get edited options list
                    var editedOptions = editedQuestion.EditedOptions.Where(e => e.EditType == EditType.Edited);

                    // deal with option edits
                    foreach (var editableOption in editedOptions)
                    {
                        if (editableOption.NewCorrectStatus != editableOption.OriginalCorrectStatus ||
                            editableOption.NewOptionLetter != editableOption.OriginalOptionLetter ||
                            editableOption.NewOptionText.Trim() != editableOption.OriginalOptionText.Trim())
                        {
                            var questionWithOption =
                                question.QuestionWithOptions.SingleOrDefault(qwo => qwo.Id == editableOption.OptionId);
                            if (ReferenceEquals(null, questionWithOption)) continue;

                            if (editableOption.NewCorrectStatus != editableOption.OriginalCorrectStatus)
                            {
                                questionWithOption.CorrectAnswer = editableOption.NewCorrectStatus;
                            }

                            if (editableOption.NewOptionLetter != editableOption.OriginalOptionLetter)
                            {
                                questionWithOption.Letter = editableOption.NewOptionLetter;
                            }

                            if (editableOption.NewOptionText.Trim() != editableOption.OriginalOptionText.Trim())
                            {
                                questionWithOption.Option.Text = editableOption.NewOptionText.Trim();
                            }
                        }
                    }

                    // deal with option deletions
                    foreach (var deletedOption in deletedOptions)
                    {
                        var questionWithOption =
                            question.QuestionWithOptions.SingleOrDefault(q => q.Id == deletedOption.OptionId);

                        if (!ReferenceEquals(null, questionWithOption))
                        {
                            question.QuestionWithOptions.Remove(questionWithOption);
                        }

                        _quizRepository.DeleteQuizWithOption(questionWithOption);
                    }

                    // deal with option additions
                    foreach (var addedOption in addedOptions)
                    {
                        var newOption = new Option { Text = addedOption.NewOptionText };

                        var questionWithOption = new QuestionWithOption
                        {
                            CorrectAnswer = addedOption.NewCorrectStatus,
                            Option = newOption,
                            idQuestion = question.Id,
                            Letter = addedOption.NewOptionLetter,
                        };

                        question.QuestionWithOptions.Add(questionWithOption);
                    }
                }

                if (!ReferenceEquals(null, question))
                {
                    if (editedQuestion.NewQuestionNumber != editedQuestion.OriginalQuestionNumber)
                    {
                        var quizWithQuestion = question.QuizWithQuestions.SingleOrDefault(q => q.idQuestion == question.Id && q.idQuiz == quizId);

                        if (!ReferenceEquals(null, quizWithQuestion))
                        {
                            quizWithQuestion.QuestionNumber = editedQuestion.NewQuestionNumber;
                        }
                    }

                    if (editedQuestion.NewQuestionText != editedQuestion.OriginalQuestionText)
                    {
                        question.Text = editedQuestion.NewQuestionText;
                    }
                }
            }
            _quizRepository.SaveChanges();

            return true;
        }

        public int CloneQuizForWebinar(Webinar webinar, int existingQuizId)
        {
            var quiz = _quizRepository.GetQuizByIdWithOptionsText(existingQuizId);
            return _quizRepository.CloneQuiz(quiz, webinar.idWebinar);
        }

        public IEnumerable<Webinar> GetByTopic(int topicId)
        {
            return _webinarRepository.GetByTopic(topicId);
        }

        public Quiz GetQuizByWebinarId(int idWebinar)
        {
            return _quizRepository.GetQuizByWebinarId(idWebinar);
        }

        public int GetQuizIdByWebinarId(int idWebinar)
        {
            return _quizRepository.GetQuizIdByWebinarId(idWebinar);
        }

        public IList<Webinar> GetSearchDTO(string searchTerm)
        {
            return _webinarRepository.GetSearchDTO(searchTerm);
        }

        public int? GetNextCompliancePerspectives()
        {
            return _webinarRepository.GetNextCompliancePerspectives();
        }

        public IList<Webinar> GetWebinarsForWeeklyInvoices(DateTime startDate)
        {

            return _webinarRepository.GetWebinarsForWeeklyInvoice(startDate);
        }

        public IEnumerable<Webinar> GetRelated(int? idWebinar)
        {
            return _webinarRepository.GetRelated(idWebinar);
        }

        public IEnumerable<Webinar> GetTopicsByWebinar(int? idWebinar)
        {
            return _webinarRepository.GetTopicsByWebinar(idWebinar);
        }

        public int ImportLu(WebinarLU importWebinar)
        {
            _logger.Info("Importing LU webinar: " + importWebinar.name);

            var theYear = importWebinar.name[0].ToString() + importWebinar.name[1].ToString();
            var theMonth = importWebinar.name[2].ToString() + importWebinar.name[3].ToString();
            var makeDate = theMonth + "/01/" + theYear;

            DateTime theDate = DateTime.Parse(Convert.ToInt32(theMonth) + "/01/" + Convert.ToInt32(theYear));
            var title = "";
            foreach (var item in importWebinar.name.Split(' ').Skip(1))
            {
                title += item + " ";
            }
            var fullName = ParseForSpeakerName(importWebinar.description_text);
            int? idPresenter = _webUserRepository.GetUserIdByFirstNameLastName(fullName);
            if (idPresenter != null && idPresenter.Value == 0)
            {
                _logger.Warn("Import LU webinar did not find speaker: " + importWebinar.name + " " + fullName);   //return 0;
                idPresenter = 24;
            }

            var learnBody = ParseLearnBody(importWebinar.description_html);
            var descBody = ParseDescBody(importWebinar.description_html);

            var newWebinar = new Webinar
            {
                Title = title.TrimEnd(' '),
                Date = theDate,
                idPresenter = idPresenter.Value,
                DescriptionLong = descBody,
                LearnBody = learnBody,
                ImageUrl = "",
                SmallImageUrl = "",
                Status = WebinarStatus.Recorded,
                LearnCaption = "Covered Topics:",
                WhoAttend = "",
                Duration = .5M,
                RecordingUrl = "",
                WebinarKey = "",
                OrganizerKey = "",
                OrganizerOAuthKey = "",
                ceu = importWebinar.minute_length + " Minutes Duration|",
                ConnectionInfo = "",
                DateCreated = DateTime.Now,
                DateChanged = DateTime.Now,
                CitrixRegisterUrl = "",
                AccessPhone = "",
                AccessCodeAttendee = "",
                AccessCodePresenter = "",
                AccessCodeOrganizer = "",
                LivePlusFiveValue = DateTime.Now,
                SeriesInfo = "DES",
                AdditionalLocationPrice = 0,
                TitleAnnouncement = "",
                Campaigns = "",
                Description = descBody
            };
            try
            {
                AddWebinar(newWebinar);
                var webinar = GetAllActive().Where(w => w.Title == title.TrimEnd(' ')).FirstOrDefault();

                _logger.Info("Added LU webinar: " + importWebinar.name + " " + fullName + " " + idPresenter);

                //Debug.Assert(webinar != null, "webinar != null");
                //webinar.RegTypesGroupsXref.Add(new RegTypesGroupsXref { idRegTypeGroup = 64 });
                var dataOperations = new DataOperations(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);


                dataOperations.insertRegTypeId(webinar.idWebinar);
                webinar.WebinarTopicXrefs.Add(new WebinarTopicXref { idTopic = 32 });

                UpdateWebinar(webinar);
                return webinar.idWebinar;

            }
            catch (Exception e)
            {
                _logger.FatalException("Unable to add: " + importWebinar.name + " " + fullName + " " + idPresenter, e);
                return 0;

            }
        }

        private string ParseLearnBody(string importWebinarDescriptionText)
        {
            try
            {
                var startPos = importWebinarDescriptionText.IndexOf("<p><u><strong>Covered Topics</strong></u></p>\r\n\r\n");
                if (startPos == -1)
                {
                    startPos = importWebinarDescriptionText.IndexOf("<p><u><strong>Covered Topics:</strong></u></p>\r\n\r\n");
                    var length = importWebinarDescriptionText.IndexOf("<p><u><strong>About the Speaker") - startPos;
                    return
                        importWebinarDescriptionText.Substring(startPos, length)
                            .Replace("<p><u><strong>Covered Topics:</strong></u></p>\r\n\r\n", "");

                }
                else
                {
                    var length = importWebinarDescriptionText.IndexOf("<p><u><strong>About the Speaker") - startPos;
                    return
                        importWebinarDescriptionText.Substring(startPos, length)
                            .Replace("<p><u><strong>Covered Topics</strong></u></p>\r\n\r\n", "");
                }
            }
            catch (Exception e)
            {
                _logger.Fatal("ParseLearnBody failed!");
                return "";
            }
        }
        private string ParseDescBody(string importWebinarDescriptionText)
        {
            try
            {

                var startPos = 0;

                var length = importWebinarDescriptionText.IndexOf("<p><u><strong>Covered") - startPos;
                return
                    importWebinarDescriptionText.Substring(startPos, length);
                //.Replace("<p><u><strong>Covered Topics</strong></u></p>\r\n\r\n", "");

            }
            catch (Exception e)
            {
                _logger.Fatal("ParseLearnBody failed!");
                return "";
            }
        }


        public string ImportLUEvents()
        {
            //https://directorseries.learnupon.com/api/v1/courses

            // Create a request using a URL that can receive a post.   
            //WebRequest request = WebRequest.Create("https://directorseries.learnupon.com/api/v1/courses");
            System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"https://directorseries.learnupon.com/api/v1/courses");

            // Set the Method property of the request to POST.  
            request.Method = "GET";
            request.Headers.Add("Authorization", "Basic NjNmOWJjYjNmYzFjNjk2OGM2NDM6NjQ0OWZkY2UzNzllOTBhZjhiNzUxOTFmMjcyMWI4");
            // Set the ContentType property of the WebRequest.  
            request.ContentType = "application/json";

            // Get the request stream.  
            Stream dataStream;

            // Get the response.  
            WebResponse response = request.GetResponse();
            // Display the status.  
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.  
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.  
            string responseFromServer = reader.ReadToEnd();
            // Display the content. 
            List<WebinarLU> luwebinars =
                JsonConvert.DeserializeObject<List<WebinarLU>>(responseFromServer.Replace("{\"courses\":", "").Replace("]}", "]"));
            // Clean up the streams.  
            reader.Close();
            dataStream.Close();
            response.Close();
            IEnumerable<Webinar> webinars = GetAllActive();
            List<WebinarLU> importEm = new List<WebinarLU>();

            foreach (var webinarLu in luwebinars)
            {
                var foundIt = webinarLu.id;
                var title = "";
                foreach (var item in webinarLu.name.Split(' ').Skip(1))
                {
                    title += item + " ";
                }
                var enumerable = webinars as Webinar[] ?? webinars.ToArray();
                var matchedIt = enumerable.Where(w => w.Title == title.TrimEnd(' ')).Select(w => w.idWebinar).FirstOrDefault();

                if (matchedIt == 0)
                    importEm.Add(webinarLu);
                //if (webinarLu.name)
            }
            var finding = new StringBuilder();
            foreach (WebinarLU w in importEm)

            {

                var fullName = ParseForSpeakerName(w.description_text);
                int? idPresenter = GetUserIdByFirstNameLastName(fullName);

                finding.AppendLine(w.name + " " + idPresenter);

                ImportLu(w);
            }
            //var notInDb = from webinarLu in luwebinars
            //    join webinar in webinars on webinarLu.name equals webinar.Title
            //    select webinarLu;

            return finding.ToString();

        }

        public string GetCampaignsByAffiliate(int affiliateId, int idWebinar)
        {

            var campaign = "";
            try
            {
                var _campaigns = GetAllActive().Where(w => w.idWebinar == idWebinar).SingleOrDefault()
                    .Campaigns;
                var o = JToken.Parse(_campaigns);
                var campaigns = JsonConvert.DeserializeObject<IEnumerable<McCampaign>>(o.First.Children().First().ToString());

                if (!campaigns.Any())
                {
                    _logger.Info("GetAffCampaign: no affiliate matched: " + affiliateId + " webinar: " + idWebinar);
                    campaign = "not found";
                    return campaign;
                }

                foreach (var campaign1 in campaigns)
                {
                    if (campaign1.AffiliateId == affiliateId)
                    {
                        campaign = campaign1.CampaignId;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return campaign;
        }

        public string ParseForSpeakerName(string importWebinarDescriptionText)
        {

            var firstName = importWebinarDescriptionText.IndexOf("About the Speaker\r\n\r\n");

            return importWebinarDescriptionText.Substring(firstName).Replace("About the Speaker\r\n\r\n", "").Split(' ')[0] + " " + importWebinarDescriptionText.Substring(firstName).Replace("About the Speaker\r\n\r\n", "").Split(' ')[1];
        }

        public int? GetUserIdByFirstNameLastName(string fullName)
        {
            return _webUserRepository.GetUserIdByFirstNameLastName(fullName);
        }

        public Quiz GetQuizByOrderId(int idOrder)
        {
            return _quizRepository.GetQuizFromOrder(idOrder);
        }

        public void UpdateWebinarFiles(IEnumerable<WebinarFile> webinarFiles)
        {
            _webinarFileRepository.UpdateRange(webinarFiles);
        }

        public Webinar GetCompliancePerspectives()
        {
            GetRegistrants_CP();
            return null;
        }

        public int GetRegTypeByLableAndWebinar(string registrationType, int idWebinar)
        {
            var regType = _webinarRepository.GetRegTypeByLableAndWebinar(registrationType, idWebinar);
            return regType;
        }

        public int GetRegTypeByRateWatch(string registrationType, int idWebinar)
        {
            return _webinarRepository.GetRegTypeByRateWatch(registrationType, idWebinar);
        }

        public int GetRegTypeByACS(string registrationType, int idWebinar)
        {
            return _webinarRepository.GetRegTypeByACS(registrationType, idWebinar);
        }

        private IList<WebUser> GetRegistrants_CP()
        {
            return null;

        }

        public IEnumerable<Webinar> GetDesWebinars()
        {
            return _webinarRepository.GetDes().ToList();
        }

        public IEnumerable<Webinar> GetCcsWebinars()
        {
            return _webinarRepository.GetCcs().ToList();
        }

        public IEnumerable<Topic> GetTopicsPerWebinar(int idWebinar)
        {
            return _webinarRepository.GetTopicsPerWebinar(idWebinar).ToList();
        }
        public IEnumerable<WebinarFile> GetWebinarFilesPerWebinar(int idWebinar)
        {
            return _webinarRepository.GetWebinarFilesPerWebinar(idWebinar).ToList();
        }
        public IEnumerable<Webinar> GetRecordedWebinars()
        {
            return _webinarRepository.GetRecorded().ToList();
        }
        public IEnumerable<Webinar> GetUpcomingWebinars()
        {
            return _webinarRepository.GetUpcoming().ToList();
        }

        public IEnumerable<RegTypesGroup> GetRegTypeGroupsForWebinars(int idWebinar)
        {
            return _webinarRepository.GetRegTypeGroupsForWebinars(idWebinar);
        }

        public IEnumerable<RegTypesGroup> GetUpcomingRegTypesForWebinars()
        {
            return _webinarRepository.GetUpcomingRegTypesForWebinars();
        }

        public Webinar GetWebinar(int id)
        {
            return _webinarRepository.FindByIdLoaded(id);
        }

        public Webinar GetWebinarThin(int id)
        {
            return _webinarRepository.FindById(id);
        }

        public IEnumerable<Webinar> GetWebinarByPresenterLastName(string lastName)
        {
            return _webinarRepository.FindByPresenterLastName(lastName);
        }

        public IEnumerable<Webinar> GetWebinarsByPresenterFullName(string searchTerm)
        {
            return _webinarRepository.FindByPresenterFullName(searchTerm);
        }

        public IEnumerable<Webinar> GetWebinarByDescription(string topicDescription)
        {
            return _webinarRepository.FindByDescription(topicDescription);
        }

        public WebinarFile GetWebinarFile(int idWebinarFile)
        {
            return _webinarFileRepository.FindById(idWebinarFile);
        }

        public Webinar GetWebinarByIdIncludingAllWebinarsByPresenter(int id)
        {
            try
            {
                return _webinarRepository.GetWebinarByIdIncludingAllWebinarsByPresenter(id);
                //return webinar;
            }
            catch (Exception exception)
            {
                _logger.ErrorException("Failed attempt to retrieve webinar with id: " + id.ToString(), exception);
                return null;
            }

        }

        public int SaveChanges()
        {
            return _webinarRepository.SaveChanges();
        }


        public IDictionary<RegType, bool> FindRegTypesByWebinarId(int webinarId)
        {
            return _regTypeRepository.FindRegTypesByWebinarId(webinarId, false);
        }

        //public IEnumerable<AdditionalLocationsLookupPrice> GetAdditionalLocationsLookupPricesForWebinar(int idWebinar)
        //{
        //    return _webinarRepository.GetAdditionalLocationsLookupPricesForWebinar(idWebinar);
        //}

        public IEnumerable<IEvent> GetEvents()
        {
            return _events;
        }

        public void Clear()
        {
            _events.Clear();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();


                _regTypeRepository.Dispose();
                _webinarRepository.Dispose();
                _webUserRepository.Dispose();

                _disposed = true;
            }
        }



        public static string CreateCalendarEvent(string title, string body, DateTime startDate, double duration, string location, string organizer, string eventId, bool allDayEvent)
        {
            // mandatory for outlook 2007
            if (String.IsNullOrEmpty(organizer))
                throw new Exception("Organizer provided was null");

            var iCal = new iCalendar
            {
                Method = "PUBLISH",
                Version = "2.0"
            };

            // "REQUEST" will update an existing event with the same UID (Unique ID) and a newer time stamp.
            //if (updatePreviousEvent)
            //{
            //    iCal.Method = "REQUEST";
            //}

            var evt = iCal.Create<Event>();
            evt.Summary = title;
            evt.Start = new iCalDateTime(startDate);
            evt.Duration = TimeSpan.FromHours(duration);
            evt.Description = body;
            evt.Location = location;
            evt.IsAllDay = allDayEvent;
            evt.UID = String.IsNullOrEmpty(eventId) ? new Guid().ToString() : eventId;
            evt.Organizer = new Organizer(organizer);
            evt.Alarms.Add(new Alarm
            {
                Duration = new TimeSpan(0, 15, 0),
                Trigger = new Trigger(new TimeSpan(0, 15, 0)),
                Action = AlarmAction.Display,
                Description = "Reminder"
            });

            return new iCalendarSerializer().SerializeToString(iCal);
        }


        public IEnumerable<Webinar> GetAllActive()
        {
            try
            {
                return _webinarRepository.GetAllActive().ToList();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("GetAllActive method", exception);
                throw;
            }
        }

        public void UpdateWebinar(Webinar webinar)
        {
            _updateWebinarValidator.ValidateAndThrow(webinar);
            _webinarRepository.Update(webinar);
        }
    }


}

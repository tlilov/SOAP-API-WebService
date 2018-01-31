using System;
using System.Collections.Generic;
using System.Linq;
using EmailSystem.com.bronto.api;
using EmailSystem.Configuration;
using EmailSystem.Models;
using EmailSystem.Interfaces;

namespace EmailSystem
{
    public class BrontoEmailService : IEmailService
    {
        private ILogger Logger { get; set; }
        private BrontoTranslator translator;
        private IBrontoApiWrapper wrapper;
        private IConfigurationRepository configRepo;
        private Dictionary<string, string> contactFields { get; set; }
        private Dictionary<string, string> templates { get; set; }
        private string siteCode;
        private Site Site;

        public BrontoEmailService(ILogger logger,
            IBrontoApiWrapper wrapper,
            IConfigurationRepository configRepo,
            Site site)
        {

            this.Logger = logger;
            this.siteCode = site.SiteCode;
            this.templates = new Dictionary<string, string>();
            this.wrapper = wrapper;
            this.configRepo = configRepo;

            logger.Debug("Instantiating BrontoEmailServiceFor: " + site.SiteCode);

            Login(siteCode);
            this.Site = site;
        }

        private void Login(string siteCode)
        {
            try
            {
                var apiKey = configRepo.GetApiKey(siteCode.ToUpper());

                Logger.Debug(String.Format("Using Bronto API Key: '{0}'", apiKey));

                this.translator = new BrontoTranslator();

                if (wrapper.SessionHeaderValue == null)
                {
                    var header = new sessionHeader();
                    header.sessionId = wrapper.Login(apiKey);
                    wrapper.SessionHeaderValue = header;

                    Logger.Debug(String.Format("Bronto Session ID: '{0}'", header.sessionId));
                }
            }
            catch (Exception e)
            {
                Logger.Error("Could not connect to Bronto API");
                Logger.Error(e);
                throw e;
            }
        }

        public string AddContact(Contact contact)
        {
            if (contact == null)
            {
                Logger.Error("Call to AddContact() when contact was null.");
                throw new ArgumentNullException("contact");
            }

            Logger.Debug("AddContact(): Email " + contact.Email);
            string errorString;
            try
            {
                List<contactObject> contacts = new List<contactObject>();
                contact.Email = contact.Email.ToLower();

                // I don't understand why we have this concept
                // of a default contact status. Apparently it has
                // something to do with VG, but it doesn't belong 
                // here... it should be in the 1CB code. 
                if (Site != null && contact.Status != ContactStatusEnum.Onboarding)
                {
                    contact.Status = (ContactStatusEnum)Enum.Parse(typeof(ContactStatusEnum), Site.DefaultContactStatus, true);
                }

                contacts.Add(translator.TranslateContact(contact, GetContactFields()));

                var result = wrapper.AddContacts(contacts.ToArray());

                if (!result.results[0].isError)
                {
                    Logger.Debug(String.Format("AddContact Function: New Contact with Email: '{0}' added with status on Bronto set to: '{1}'", contact.Email, contact.Status));
                    return result.results[0].id;
                }
                else
                {
                    errorString = result.results[0].errorString;
                    Logger.Error(errorString);

                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw e;
            }

            Logger.Debug("Leaving AddContact()");
            return errorString;
        }

        public void UpdateContact(Contact contact)
        {
            if (contact == null)
            {
                Logger.Error("Call to UpdateContact() when contact was null.");
                throw new ArgumentNullException("contact");
            }

            var contacts = new List<contactObject>();

            // I don't know why we are forcing the email to lower-case,
            // but apparently that is what we need.
            contact.Email = contact.Email.ToLower();
            var brontoContact = translator.TranslateContact(contact, GetContactFields());

            if (ApiStatusUpdateIsDisabled())
                brontoContact.status = null;

            Logger.Debug("Updating contact with email address of " + contact.Email);

            contacts.Add(brontoContact);
            wrapper.UpdateContacts(contacts.ToArray());
        }

        private bool ApiStatusUpdateIsDisabled()
        {
            return System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("PreventApiStatusUpdate")
                && bool.Parse(System.Configuration.ConfigurationManager.AppSettings["PreventApiStatusUpdate"]);
        }

        public void UpdateContact(string oldEmail, Contact newContactInfo)
        {
            // get the contactId for the old email address
            var contact = GetContact(oldEmail);

            if (contact == null)
                throw new Exception("Contact not found to update.");

            Logger.Debug("Updating email address from " + oldEmail + " to " + newContactInfo.Email);
            newContactInfo.Id = contact.Id;
            UpdateContact(newContactInfo);
        }

        public ServiceResponse AddOrderData(OrderData orderData)
        {
            if (orderData == null)
            {
                Logger.Error("Call to AddOrderData() when orderData was null.");
                throw new ArgumentNullException("orderData");
            }

            try
            {
                var response = new ServiceResponse();
                var orderObjects = translator.TranslateOrder(orderData);
                var result = wrapper.AddOrUpdateOrders(orderObjects);

                foreach (resultItem ri in result.results)
                {
                    if (ri.isError)
                    {
                        response.Errors.AddError(ri.errorCode + " : " + ri.errorString);
                    }
                }

                return response;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw e;
            }
        }

        private Dictionary<string, string> GetContactFields()
        {
            try
            {
                if (this.contactFields == null)
                {
                    this.contactFields = new Dictionary<string, string>();

                    var filt = new fieldsFilter();

                    var fields = wrapper.ReadFields(filt);

                    if (fields != null)
                    {
                        foreach (fieldObject field in fields)
                        {
                            this.contactFields.Add(field.id, field.name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw e;
            }

            return this.contactFields;
        }

        private string[] GetContactFieldIds()
        {
            var fields = GetContactFields();

            if (fields == null)
            {
                Logger.Error("Contact fields is null inside GetContactFieldIds()");
            }

            return GetContactFields().Keys.ToArray();
        }

        public Contact GetContact(string contactEmail)
        {
            if (String.IsNullOrEmpty(contactEmail))
            {
                Logger.Error("Call to GetContact() when contactEmail was null.");
                throw new ArgumentNullException("contactEmail");
            }

            try
            {
                List<stringValue> emailVals = new List<stringValue>();
                emailVals.Add(new stringValue { value = contactEmail, @operator = filterOperator.EqualTo, operatorSpecified = true });
                var filter = new contactFilter { email = emailVals.ToArray() };
                var brontoContacts = wrapper.ReadContacts(filter, GetContactFieldIds());

                if (brontoContacts != null && brontoContacts.Length > 0)
                {
                    var brontoContact = brontoContacts[0];
                    var ct = new Contact()
                    {
                        Id = brontoContact.id,
                        Email = brontoContact.email,
                        SiteCode = this.siteCode
                    };

                    if (brontoContact.fields != null && brontoContact.fields.Length > 0)
                    {
                        foreach (contactField f in brontoContact.fields)
                        {
                            if (!ct.Fields.ContainsKey(GetContactFields()[f.fieldId]))
                            {
                                ct.Fields.Add(GetContactFields()[f.fieldId], f.content);
                            }
                            else
                            {
                                ct.Fields[GetContactFields()[f.fieldId]] = f.content;
                            }

                        }
                    }

                    ct.Status = (ContactStatusEnum)Enum.Parse(typeof(ContactStatusEnum), brontoContact.status, true);
                    Logger.Debug(String.Format("GetContact Function: Contact with email: '{0}' has status on Bronto: '{1}'", contactEmail, brontoContact.status));
                    return ct;
                }
                else
                {
                    Logger.Debug(String.Format("GetContact Function: Contact with email: '{0}' was not found on Bronto", contactEmail));
                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw e;
            }
        }

        private string GetListId(string listName)
        {
            try
            {
                var filter = new mailListFilter();
                var filterName = new List<stringValue>();

                filterName.Add(new stringValue { value = listName, @operator = filterOperator.Contains });
                filter.name = filterName.ToArray();

                mailListObject[] lists = wrapper.ReadLists(filter);

                return lists.FirstOrDefault().id;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw e;
            }
        }

        private string GetTemplateId(string messageName)
        {

            try
            {
                if (!this.templates.ContainsKey(messageName))
                {
                    List<stringValue> templateVals = new List<stringValue>();
                    templateVals.Add(new stringValue { value = messageName, @operator = filterOperator.EqualTo, operatorSpecified = true });
                    var filter = new messageFilter { name = templateVals.ToArray() };
                    var brontoTemplates = wrapper.ReadMessages(filter);

                    if (brontoTemplates != null && brontoTemplates.Length > 0)
                    {
                        this.templates.Add(messageName, brontoTemplates[0].id);
                    }
                    else
                    {
                        return null;
                    }
                }

                return this.templates[messageName];
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw e;
            }
        }

        public List<ServiceResult> SendMessage(Message message)
        {
            if (message == null)
            {
                Logger.Error("Call to SendMessage() when message was null.");
                throw new ArgumentNullException("message");
            }

            try
            {
                List<Message> messages = new List<Message>();
                messages.Add(message);

                return SendMessages(messages.ToArray());
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw e;
            }
        }

        public List<ServiceResult> SendMessages(Message[] messages)
        {
            foreach (var message in messages)
            {
                if (message.TemplateName != null)
                    message.TemplateId = GetTemplateId(message.TemplateName);

                if (message.ContactType == "list")
                {
                    message.ContactId = GetListId(message.To);
                }
                else
                {
                    var contact = GetContact(message.To);
                    string id;

                    if (contact == null)
                    {
                        id = AddContact(new Contact { Email = message.To });
                        Logger.Debug(String.Format("SendMessage '{0}' Function: After new Contact with email: '{1}' created on Bronto.", message.TemplateName, message.To));
                    }
                    else
                    {
                        id = contact.Id;
                        Logger.Debug(String.Format("SendMessage '{0}' Function: Contact email found: '{1}' has status on Bronto: '{2}'", message.TemplateName, message.To, contact.Status));
                    }

                    message.ContactId = id;
                }
            }

            var deliveries = translator.TranslateDelivery(messages);
            var sendAtOnce = Convert.ToInt32(configRepo.GetConfig("MessagesToSendAtOnce"));
            var skip = 0;
            var results = new List<ServiceResult>();

            do
            {
                var sending = deliveries.Skip(skip).Take(sendAtOnce).ToList();
                writeResult result = null;

                try
                {
                    result = wrapper.AddDeliveries(sending.ToArray());
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);

                    result = new writeResult
                    {
                        results = new resultItem[sending.Count]
                    };

                    for (var i = 0; i < sending.Count; i++)
                    {
                        result.results[i] = new resultItem()
                        {
                            id = "unknown",
                            isError = true,
                            errorCode = 0,
                            errorString = "Bronto communication issue."
                        };
                    }
                }

                foreach (var ri in result.results)
                {
                    var mr = new ServiceResult { Id = ri.id };

                    if (ri.isError)
                    {
                        mr.Error = ri.errorCode + " : " + ri.errorString;
                    }

                    results.Add(mr);
                }

                skip += sendAtOnce;
            } while (skip <= deliveries.Count());

            return results;
        }

        public Dictionary<string, string> GetMessageDeliveryStatus(string[] messageIds)
        {
            Dictionary<string, string> delStatus = new Dictionary<string, string>();

            var filter = new deliveryFilter();

            filter.id = messageIds;

            var deliveries = wrapper.ReadDeliveries(filter);

            if (deliveries != null)
            {
                string status;
                foreach (deliveryObject d in deliveries)
                {
                    if (d.bounceRate == 1.0)
                    {
                        status = "bounced";
                    }
                    else if (d.deliveryRate == 1.0)
                    {
                        status = "delivered";
                    }
                    else
                    {
                        status = d.status;
                    }

                    delStatus.Add(d.id, status);
                }
            }

            return delStatus;
        }
    }
}
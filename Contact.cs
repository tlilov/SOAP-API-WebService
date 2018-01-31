using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSystem
{

    public class Contact
    {
        public Contact()
        {
            Fields = new Dictionary<string, string>();
            Status = ContactStatusEnum.Transactional;
            this.Fields.Add("firstname", String.Empty);
            this.Fields.Add("lastname", String.Empty);
            this.Fields.Add("birthday", String.Empty);
            this.Fields.Add("CustomerIP", String.Empty);
            this.Fields.Add("OSRC", String.Empty);
            this.Fields.Add("guid", String.Empty);
            this.Fields.Add("accountType", String.Empty);
            this.Fields.Add("org_siteid", String.Empty);
            this.Fields.Add("signupSpot", String.Empty);
        }

        public string Id { get; set; }
        public string SiteCode {
            get 
            { 
                return this.Fields["org_siteid"];
            }
            set 
            {
                if (this.Fields.ContainsKey("org_siteid"))
                {
                    this.Fields["org_siteid"] = value;
                }
                else
                {
                    this.Fields.Add("org_siteid", value);
                }
            } 
        }
        public string Email { get; set; }
        public string FirstName
        {
            get
            {
                return this.Fields["firstname"];
            }
            set
            {
                if (this.Fields.ContainsKey("firstname"))
                {
                    this.Fields["firstname"] = value;
                }
                else
                {
                    this.Fields.Add("firstname", value);
                }
            }
        }
        public string LastName
        {
            get
            {
                return this.Fields["lastname"];
            }
            set
            {
                if (this.Fields.ContainsKey("lastname"))
                {
                    this.Fields["lastname"] = value;
                }
                else
                {
                    this.Fields.Add("lastname", value);
                }
            }
        }
        public DateTime BirthDate
        {
            get
            {
                DateTime dt;

                if (DateTime.TryParse(this.Fields["birthday"], out dt))
                {
                    return dt;
                }
                {
                    return new DateTime(1901, 1, 1);
                }
            }
            set
            {
                if (this.Fields.ContainsKey("birthday"))
                {
                    this.Fields["birthday"] = value.ToString("yyyy-MM-dd");
                }
                else
                {
                    this.Fields.Add("birthday", value.ToString("yyyy-MM-dd"));
                }
            }
        }
        public Dictionary<string, string> Fields;
        public bool OptedIn
        {
            get
            {
                if (Status == ContactStatusEnum.Active || Status == ContactStatusEnum.Onboarding)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            set
            {
                if (value)
                {
                    Status = ContactStatusEnum.Onboarding;
                }
                else
                {
                    Status = ContactStatusEnum.Unsub;
                }

            }
        }
        public ContactStatusEnum Status { get; set; }
        public string GetStatusString()
        {
            return Status.ToString().ToLower();
        }
        public string MessagePreference { get; set; }

        public string IP 
        {
        get 
            {
                return this.Fields["CustomerIP"];
            }
            set 
            {
                if (this.Fields.ContainsKey("CustomerIP"))
                {
                    this.Fields["CustomerIP"] = value;
                }
                else
                {
                    this.Fields.Add("CustomerIP", value);
                }
            } 
        }
        public string InitialSource
        {
            get
            {
                return this.Fields["OSRC"];
            }
            set
            {
                if (this.Fields.ContainsKey("OSRC"))
                {
                    this.Fields["OSRC"] = value;
                }
                else
                {
                    this.Fields.Add("OSRC", value);
                }
            }
        }
        public string Spot
        {
            get
            {
                return this.Fields["signupSpot"];
            }
            set
            {
                if (this.Fields.ContainsKey("signupSpot"))
                {
                    this.Fields["signupSpot"] = value;
                }
                else
                {
                    this.Fields.Add("signupSpot", value);
                }
            }
        }
        public string GUID
        {
            get
            {
                return this.Fields["guid"];
            }
            set
            {
                if (this.Fields.ContainsKey("guid"))
                {
                    this.Fields["guid"] = value;
                }
                else
                {
                    this.Fields.Add("guid", value);
                }
            }
        }
        public Int32 AccountType
        {
            get
            {
                Int32 i;

                if (Int32.TryParse(this.Fields["accountType"], out i))
                {
                    return i;
                }
                {
                    return 0;
                }
            }
            set
            {
                if (this.Fields.ContainsKey("accountType"))
                {
                    this.Fields["accountType"] = value.ToString();
                }
                else
                {
                    this.Fields.Add("accountType", value.ToString());
                }
            }
        }
    }

    public enum ContactStatusEnum
    {
        Active = 1,
        Onboarding = 2,
        Transactional = 3,
        Bounce = 4,
        Unconfirmed = 5,
        Unsub = 6
    }
}

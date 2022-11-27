using System;
using System.Globalization;
using System.Text.RegularExpressions;
namespace FD.Networking.Database.Entities.Account
{
    public class EmailAddress
    {
        public string Local;
        public string Site;
        public string Domain;


        public EmailAddress()
        {

        }

        public EmailAddress(string local, string site, string domain)
        {
            Local = local;
            Site = site;
            Domain = domain;
        }



        #region Obsolete - use static method Parse or TryParse (TryParse recomended)
        //public EmailAddress(string fullAddress)
        //{
        //    #region velociped
        //    //StringBuilder sb = new(fullAddress.Length);
        //    //for (int i = 0; i < fullAddress.Length; i++)
        //    //{
        //    //    char c = fullAddress[i];
        //    //    if (c == '@')
        //    //        break;

        //    //    sb.Append(c);
        //    //}

        //    //Login = sb.ToString();

        //    //sb = new StringBuilder(fullAddress.Length - Login.Length);

        //    //for (int i = Login.Length + 1; i < fullAddress.Length; i++)
        //    //{
        //    //    char c = fullAddress[i];

        //    //    if (c == '.')
        //    //        break;

        //    //    sb.Append(c);
        //    //}

        //    //Site = sb.ToString();

        //    //Domen = fullAddress.
        //    #endregion


        //}
        #endregion

        public override string ToString()
        {
            return $"{Local}@{Site}.{Domain}";
        }

        // Ниже замена, которая выдавала рандомную ошибку в другой версии.
        //public static bool IsValidEmail(string address)
        //{
        //    var pos = address.LastIndexOf("@");
        //    return pos > 0 && (address.LastIndexOf(".") > pos) && (address.Length - pos > 4) && (address.Length < 254);
        //}

        public static bool TryParse(string source, out EmailAddress parsed)
        {
            if (!IsValidEmail(source))
            {
                parsed = null;
                return false;
            }

            try
            {
                parsed = Parse(source);
                return true;
            }
            catch (Exception)
            {
                parsed = null;
                return false;
            }
        }

        public static EmailAddress Parse(string email)
        {
            try
            {
                var e = new EmailAddress();
                var loginAndOther = email.Split('@', 2);
                e.Local = loginAndOther[0];
                var siteAndDomen = loginAndOther[1].Split('.', 2);
                e.Site = siteAndDomen[0];
                e.Domain = siteAndDomen[1];
                return e;
            }
            catch (IndexOutOfRangeException ioore)
            {
                throw new Exception($"Email parsing of value '{email}' failed : {ioore.Message}");
            }

        }

        #region broken (?) example
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        #endregion

    }
}

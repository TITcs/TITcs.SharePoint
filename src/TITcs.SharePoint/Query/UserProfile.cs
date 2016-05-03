using System;
using System.Linq;
using Microsoft.Office.Server;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;

namespace TITcs.SharePoint.Query
{
    public class UserProfile
    {
        public static UserProfileInfo GetCurrent()
        {
            try
            {
                var query = ContextFactory.GetContextWeb();

                if (query.IsClient)
                {
                    var context = (ClientContext) query.Context;

                    var peopleManager = new Microsoft.SharePoint.Client.UserProfiles.PeopleManager(context);

                    Microsoft.SharePoint.Client.UserProfiles.PersonProperties personDetails =
                        peopleManager.GetMyProperties();

                    context.Load(personDetails,
                        personsD => personsD.AccountName,
                        personsD => personsD.Email,
                        personsD => personsD.Title,
                        personsD => personsD.PictureUrl,
                        personsD => personsD.UserProfileProperties,
                        personsD => personsD.DisplayName);

                    context.ExecuteQuery();

                    return new UserProfileInfo
                    {
                        Department = personDetails.UserProfileProperties["Department"],
                        DisplayName = personDetails.DisplayName,
                        Email = personDetails.Email,
                        PictureUrl = personDetails.PictureUrl,
                        Title = personDetails.Title,
                        AccountName = personDetails.AccountName,
                        Properties = personDetails.UserProfileProperties
                    };
                }
                else
                {
                    //string currentUser = SPContext.Current.Web.CurrentUser.LoginName;
                    //SPSite spSite = SPContext.Current.Site;

                    var context = (SPWeb)query.Context;

                    SPServiceContext serviceContext = SPServiceContext.GetContext(context.Site);

                    var profileManager = new UserProfileManager(serviceContext);
                    var userProfile = profileManager.GetUserProfile(context.CurrentUser.LoginName);

                    return new UserProfileInfo
                    {
                        AccountName = userProfile.AccountName,
                        DisplayName = userProfile.DisplayName,
                        Department = userProfile["Department"].Value.ToString(),
                        Email = userProfile["Email"].Value.ToString(),
                        PictureUrl = userProfile["PictureUrl"].Value.ToString(),
                        Title = userProfile["Title"].Value.ToString(),
                        Properties = userProfile.Properties.ToDictionary(p => p.Name, p => p.Name)
                    };
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }

    }

}

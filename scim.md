# Capturi SCIM integration

## TLDR 

Integration between an SCIM compatible authentication provider and capturi, so that users can manage (give, revoke) 
access using for example azure AD. 

The integration have been developed for Azure AD and is therefore not tested on other SCIM providers.


## What's possible

* Sync users (create, disable)
* Sync and create teams and team members (create and delete teams)
* Sync team leads
* Sync administrators

As a rule of thumb, a user should only be in one AD group. The exception is team leads which can be in several groups.

## Configuration / Setup

1. login to azure portal https://portal.azure.com/#home
2. Open "Microsoft Entra ID"
3. Select "Enterprise applications in the left menu"
4. Click "New Application" in the top menu
5. Click "Create your own application" in the top menu
6. Give the application a name and select "Integrate any other application you don't find in the gallery (Non-gallery)" and click "create"
7. Select "Provisioning" in the left menu
8. Select "Provisioning" again
9. Set provisioning mode to "Automatic"
10. Open "Admin credentials"
11. Tenant url should be "https://scim.capturi.ai/?aadOptscim062020"
12. Secret token is the generated and supplied by Capturi, if you haven't got one. Contact your Capturi Customer Success Manager
13. Click on test connection and then on save.
14. Open mappings and click on "Provision Azure Active Directory Users"
15. Remove the following rows
    1. jobtitle
    2. preferredLanguage
    3. givenName
    4. surname
    5. Join(" ", [givenName], [surname])
    6. physicalDeliveryOfficeName
    7. streetAddress
    8. city
    9. state
    10. postalCode
    11. country
    12. telephoneNumber
    13. mobile
    14. facsimileTelephoneNumber
    15. employeeId
    16. department
    17. manager
16. Click on "mailNickname"
17. Click on the "Source attribute" dropdown and select "objectId" and the "Ok"
18. Click "Save" in the top menu
19. Under settings make sure that "Scope" is "Sync only assigned users and groups"
20. Refresh the page -> "Users and groups" should appear in the left menu. Click on "Users and groups"
21. Add the users and groups that you want synced to Capturi. (See section below for naming conventions)
22. Open overview in the left menu and select "Start provisioning"


Keep an eye on the status and the logs.

## Naming convention and user / groups setup

To be able to match the access model in Capturi (Link: "https://capturi.stonly.com/kb/guide/en/roles-Kci4BwzWcu/Steps/1655497") users with access to Capturi needs to be in AD **Security Groups** matching their role and permissions.

Note. Users can only have one role at a time.

Group names syncing to capturi must follow the following naming conventions:

Administative roles:

* To give users administrative role, add them to a group with the following name: '**capturi_role_admins**'  
* To give users owner role, add them to a group with the following name: '**capturi_role_owners**'  

For users not in Teams:

* Add them to a group with the following format: '**capturi_users_description**' (eg: capturi_this-is-capturi-users)

For users in Teams:

* Add users to a group with the following format: '**capturi_team_teamName**' (Teams will be created in Capturi with the name provided)
* Set users as team lead for team,  Add users to a group with the following format: '**capturi_teamlead_teamName_teamExternalId**' (teamExternalId is found under the team group. Click on the group in Entra Id, and copy the "object Id" field)

 



﻿// -------------------------------------------------------------------------
//  Copyright © 2021 Province of British Columbia
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  https://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// -------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EMBC.ESS.Resources.Cases;
using EMBC.ESS.Resources.Contacts;
using EMBC.ESS.Shared.Contracts.Submissions;
using EMBC.ESS.Utilities.Notifications;
using EMBC.ESS.Utilities.Transformation;

namespace EMBC.ESS.Managers.Submissions
{
    public class SubmissionsManager
    {
        private readonly IMapper mapper;
        private readonly IContactRepository contactRepository;
        private readonly ITemplateProviderResolver templateProviderResolver;
        private readonly ICaseRepository caseRepository;
        private readonly ITransformator transformator;
        private readonly INotificationSender notificationSender;

        public SubmissionsManager(
            IMapper mapper,
            IContactRepository contactRepository,
            ITemplateProviderResolver templateProviderResolver,
            ICaseRepository caseRepository,
            ITransformator transformator,
            INotificationSender notificationSender)
        {
            this.mapper = mapper;
            this.contactRepository = contactRepository;
            this.templateProviderResolver = templateProviderResolver;
            this.caseRepository = caseRepository;
            this.transformator = transformator;
            this.notificationSender = notificationSender;
        }

        public async Task<string> Handle(SubmitAnonymousEvacuationFileCommand cmd)
        {
            var file = mapper.Map<Resources.Cases.EvacuationFile>(cmd.File);
            var contact = mapper.Map<Contact>(cmd.SubmitterProfile);

            file.PrimaryRegistrantId = (await contactRepository.ManageContact(new SaveContact { Contact = contact })).ContactId;

            var caseId = (await caseRepository.ManageCase(new SaveEvacuationFile { EvacuationFile = file })).CaseId;

            if (contact.Email != null)
            {
                await SendEmailNotification(
                    SubmissionTemplateType.NewAnonymousEvacuationFileSubmission,
                    email: contact.Email,
                    name: $"{contact.LastName}, {contact.FirstName}",
                    tokens: new[] { KeyValuePair.Create("fileNumber", caseId) });
            }

            return caseId;
        }

        public async Task<string> Handle(SubmitEvacuationFileCommand cmd)
        {
            var file = mapper.Map<Resources.Cases.EvacuationFile>(cmd.File);
            var contact = (await contactRepository.QueryContact(new ContactQuery { ContactId = file.PrimaryRegistrantId })).Items.SingleOrDefault();

            if (contact == null) throw new Exception($"Registrant not found '{file.PrimaryRegistrantId}'");

            var caseId = (await caseRepository.ManageCase(new SaveEvacuationFile { EvacuationFile = file })).CaseId;

            if (string.IsNullOrEmpty(file.Id) && !string.IsNullOrEmpty(contact.Email))
            {
                //notify registrant of the new file and has email
                await SendEmailNotification(
                    SubmissionTemplateType.NewEvacuationFileSubmission,
                    email: contact.Email,
                    name: $"{contact.LastName}, {contact.FirstName}",
                    tokens: new[] { KeyValuePair.Create("fileNumber", caseId) });
            }

            return caseId;
        }

        public async Task<EvacuationFilesQueryResult> Handle(EvacuationFilesQuery query)
        {
            return await Task.FromResult(new EvacuationFilesQueryResult());
        }

        public async Task<EvacuationFilesQueryResult> Handle(EvacuationFilesSearchQuery query)
        {
            return await Task.FromResult(new EvacuationFilesQueryResult());
        }

        public async Task<RegistrantsQueryResult> Handle(RegistrantsQuery query)
        {
            var items = (await contactRepository.QueryContact(new ContactQuery { UserName = query.ByUserName })).Items;

            return new RegistrantsQueryResult { Items = mapper.Map<IEnumerable<RegistrantProfile>>(items) };
        }

        public async Task<RegistrantsQueryResult> Handle(RegistrantsSearchQuery query)
        {
            return await Task.FromResult(new RegistrantsQueryResult());
        }

        public async Task<string> Handle(SaveRegistrantCommand cmd)
        {
            var contact = mapper.Map<Contact>(cmd.Profile);
            var result = await contactRepository.ManageContact(new SaveContact { Contact = contact });

            if (string.IsNullOrEmpty(cmd.Profile.Id))
            {
                //send email when creating a new registrant profile
                if (contact.Email != null)
                {
                    await SendEmailNotification(
                        SubmissionTemplateType.newProfileRegistration,
                        email: contact.Email,
                        name: $"{contact.LastName}, {contact.FirstName}",
                        tokens: Array.Empty<KeyValuePair<string, string>>());
                }
            }

            return result.ContactId;
        }

        public async Task Handle(DeleteRegistrantCommand cmd)
        {
            await contactRepository.ManageContact(new DeleteContact { ContactId = cmd.RegistrantId });
        }

        private async Task SendEmailNotification(SubmissionTemplateType notificationType, string email, string name, IEnumerable<KeyValuePair<string, string>> tokens)
        {
            var template = (EmailTemplate)await templateProviderResolver.Resolve(NotificationChannelType.Email).Get(notificationType);
            var emailContent = (await transformator.Transform(new TransformationData
            {
                Template = template.Content,
                Tokens = tokens
            })).Content;

            await notificationSender.Send(new EmailNotification
            {
                Subject = template.Subject,
                Content = emailContent,
                To = new[] { new EmailAddress { Name = name, Address = email } }
            });
        }
    }
}

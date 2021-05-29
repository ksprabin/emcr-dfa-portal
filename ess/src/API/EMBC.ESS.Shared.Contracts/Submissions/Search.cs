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

namespace EMBC.ESS.Shared.Contracts.Submissions
{
    public class EvacuationFilesSearchQuery : Query<EvacuationFilesSearchQueryResult>
    {
        public string FileId { get; set; }
        public string PrimaryRegistrantId { get; set; }
        public string PrimaryRegistrantUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public bool IncludeHouseholdMembers { get; set; }
        public EvacuationFileStatus[] IncludeFilesInStatuses { get; set; } = Array.Empty<EvacuationFileStatus>();
    }

    public class EvacuationFilesSearchQueryResult
    {
        public IEnumerable<EvacuationFile> Items { get; set; }
    }

    public class RegistrantsSearchQuery : Query<RegistrantsSearchQueryResult>
    {
        public string UserId { get; set; }
        public string FileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
    }

    public class RegistrantsSearchQueryResult
    {
        public IEnumerable<RegistrantWithFiles> Items { get; set; }
    }

    public class RegistrantWithFiles
    {
        public RegistrantProfile RegistrantProfile { get; set; }
        public IEnumerable<EvacuationFile> Files { get; set; }
    }
}

namespace EMBC.Gov.BCeID
{
    /// <summary>
    /// Source:
    /// https://github.com/bcgov/pssg-cscp-cpu/blob/92c8b09dd6a6fa982001d96aad16e549c625df22/cpu-interfaces/BCeID/BCeIDBusiness.cs
    /// </summary>
    public class BCeIDBusiness
    {
        public bool IsValidResponse { get; set; }
        public string ResponseErrorMsg { get; set; }
        public string contactEmail { get; set; }
        public string contactPhone { get; set; }

        public string individualFirstname { get; set; }
        public string individualMiddlename { get; set; }
        public string individualOtherMiddlename { get; set; }
        public string individualSurname { get; set; }

        public string businessTypeName { get; set; }
        public string businessTypeDescription { get; set; }
        public string businessTypeCode { get; set; }
        public string businessTypeOther { get; set; }
        public string legalName { get; set; }
        public string businessNumber { get; set; }
        public bool businessNumberVerified { get; set; }
        public string statementOfRegistrationNumber { get; set; }
        public string incorporationNumber { get; set; }
        public string jurisdictionOfIncorporation { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressCity { get; set; }
        public string addressProv { get; set; }
        public string addressPostal { get; set; }
        public string addressCountry { get; set; }
        public string userId { get; set; }
        public string department { get; set; }

        // 2024-09-13 EMCRI-676 waynezen
        public Guid organizationGuid { get; set; }
        public Guid userGuid { get; set; }

    }
}


/* tslint:disable */
/* eslint-disable */
import { Contact } from './contact';
export interface PdfApplicationData {
  addressLine1?: string;
  addressLine2?: string;
  businessNumber?: string;
  businessPhone?: string;
  causeofDamage?: string;
  cellPhone?: string;
  city?: string;
  contacts?: Array<Contact>;
  contactsText?: string;
  dateofDamageFrom?: string;
  dateofDamageTo?: string;
  department?: string;
  describeYourOrganization?: string;
  disasterEvent?: string;
  doingBusinessAsDBAName?: string;
  emailAddress?: string;
  firstName?: string;
  governmentType?: string;
  indigenousGoverningBody?: string;
  jobTitle?: string;
  lastName?: string;
  otherGoverningBody?: string;
  postalCode?: string;
  province?: string;
}

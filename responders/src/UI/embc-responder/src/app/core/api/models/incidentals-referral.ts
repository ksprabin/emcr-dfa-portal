/* tslint:disable */
/* eslint-disable */
import { Referral } from './referral';
import { SupportCategory } from './support-category';
import { SupportSubCategory } from './support-sub-category';
export interface IncidentalsReferral extends Referral {
  approvedItems: string;
  approverName?: string;
  category: SupportCategory;
  subCategory: SupportSubCategory;
  totalAmount: number;
}

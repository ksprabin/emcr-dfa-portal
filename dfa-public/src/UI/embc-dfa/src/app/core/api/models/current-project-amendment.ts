/* tslint:disable */
/* eslint-disable */
import { ProjectStatusBar } from './project-status-bar';
export interface CurrentProjectAmendment {
  additionalProjectCostDecision?: string;
  amended18MonthDeadline?: string;
  amendedProjectDeadlineDate?: string;
  amendmentApprovedDate?: string;
  amendmentId?: string;
  amendmentNumber?: string;
  amendmentReason?: string;
  amendmentReceivedDate?: string;
  approvedAdditionalProjectCost?: string;
  deadlineExtensionApproved?: string;
  emcrDecisionComments?: string;
  estimatedAdditionalProjectCost?: string;
  isErrorInStatus?: boolean;
  isHidden?: boolean;
  projectId?: string;
  requestforAdditionalProjectCost?: string;
  requestforProjectDeadlineExtention?: string;
  stage?: string;
  status?: string;
  statusBar?: Array<ProjectStatusBar>;
  statusColor?: string;
  statusLastUpdated?: string;
}

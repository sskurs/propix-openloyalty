import { apiClient } from './apiClient';

// This interface matches the full EarningRule entity from the backend
export interface EarningRule {
    id: string;
    name: string;
    description?: string;
    status: string;
    category: string;
    eventKey: string;
    priority: number;
    version: number;
    type: string;
    activateAt?: string;
    deactivateAt?: string;
    cronExpression?: string;
    conditionJson?: string;
    pointsJson?: string;
    limitsJson?: string;
    timeWindowJson?: string;
    segmentsJson?: string;
    metadata?: string;
    createdAt: string;
    updatedAt: string;
}

// This interface matches the backend CreateEarningRuleDto
export interface CreateEarningRulePayload {
    name: string;
    description?: string;
    status: string;
    category: string;
    eventKey: string;
    priority: number;
    condition?: object;
    points: object;
    limits?: object;
    timeWindow?: object;
    segments?: string[];
    activateAt?: string;
    deactivateAt?: string;
    cronExpression?: string;
}

export const getEarningRules = async (): Promise<EarningRule[]> => {
    return apiClient.get<EarningRule[]>('/earningrules');
};

export const createEarningRule = async (payload: CreateEarningRulePayload): Promise<EarningRule> => {
  return apiClient.post('/earningrules', payload);
};

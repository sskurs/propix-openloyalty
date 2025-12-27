"use client";

import { RuleGroup as RuleGroupType, createEmptyRuleGroup } from "@/types/rules";
import { RuleGroup } from "./RuleGroup";

interface RuleBuilderProps {
    value: RuleGroupType;
    onChange: (value: RuleGroupType) => void;
}

export function RuleBuilder({ value, onChange }: RuleBuilderProps) {
    // Initialize with empty group if value is null/undefined
    const ruleGroup = value || createEmptyRuleGroup();

    return (
        <div className="w-full">
            <div className="mb-4">
                <h3 className="text-lg font-semibold text-gray-900">Campaign Rules</h3>
                <p className="text-sm text-gray-600 mt-1">
                    Define the conditions that must be met for this campaign to apply.
                    You can create complex rules using AND/OR logic and nested groups.
                </p>
            </div>

            <RuleGroup
                group={ruleGroup}
                onChange={onChange}
            />

            {/* Helper Text */}
            <div className="mt-4">
                <p className="text-sm text-gray-600 mt-1">
                    <strong>Tip:</strong> Use AND when all conditions must be true, and OR when any condition can be true.
                    You can nest groups to create complex logic like "(A AND B) OR (C AND D)".
                </p>
            </div>
        </div>
    );
}

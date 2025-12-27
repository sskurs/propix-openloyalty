"use client";

import { Plus, Trash2 } from "lucide-react";
import { RuleGroup as RuleGroupType, RuleCondition as RuleConditionType, createEmptyRuleCondition, createEmptyRuleGroup } from "@/types/rules";
import { RuleCondition } from "./RuleCondition";

interface RuleGroupProps {
    group: RuleGroupType;
    onChange: (group: RuleGroupType) => void;
    onDelete?: () => void;
    depth?: number;
}

export function RuleGroup({ group, onChange, onDelete, depth = 0 }: RuleGroupProps) {
    const isNested = depth > 0;
    const maxDepth = 3; // Limit nesting to prevent overly complex rules

    const handleCombinatorChange = (combinator: "AND" | "OR") => {
        onChange({ ...group, combinator });
    };

    const handleAddCondition = () => {
        onChange({
            ...group,
            conditions: [...group.conditions, createEmptyRuleCondition()]
        });
    };

    const handleAddGroup = () => {
        if (depth >= maxDepth) {
            alert(`Maximum nesting depth (${maxDepth}) reached`);
            return;
        }
        onChange({
            ...group,
            groups: [...group.groups, createEmptyRuleGroup()]
        });
    };

    const handleConditionChange = (index: number, condition: RuleConditionType) => {
        const newConditions = [...group.conditions];
        newConditions[index] = condition;
        onChange({ ...group, conditions: newConditions });
    };

    const handleConditionDelete = (index: number) => {
        onChange({
            ...group,
            conditions: group.conditions.filter((_, i) => i !== index)
        });
    };

    const handleNestedGroupChange = (index: number, nestedGroup: RuleGroupType) => {
        const newGroups = [...group.groups];
        newGroups[index] = nestedGroup;
        onChange({ ...group, groups: newGroups });
    };

    const handleNestedGroupDelete = (index: number) => {
        onChange({
            ...group,
            groups: group.groups.filter((_, i) => i !== index)
        });
    };

    const isEmpty = group.conditions.length === 0 && group.groups.length === 0;

    return (
        <div
            className={`
        ${isNested ? "ml-6 mt-3 shadow-inner" : "shadow-md"}
        p-5 rounded-xl border-2
        ${isNested ? "border-amber-900/10 bg-[#0a0600]/60" : "border-amber-900/20 bg-[#0c0800]/70"}
      `}
        >
            {/* Header */}
            <div className="flex items-center justify-between mb-3">
                <div className="flex items-center gap-3">
                    {/* Combinator Toggle */}
                    <div className="flex bg-black/40 rounded-lg border border-amber-900/30 overflow-hidden shadow-sm">
                        <button
                            onClick={() => handleCombinatorChange("AND")}
                            className={`
                px-5 py-2 text-xs font-bold tracking-wider transition-all
                ${group.combinator === "AND"
                                    ? "bg-amber-700 text-white shadow-inner"
                                    : "bg-transparent text-amber-100/40 hover:bg-amber-800/10"}
              `}
                        >
                            AND
                        </button>
                        <button
                            onClick={() => handleCombinatorChange("OR")}
                            className={`
                px-5 py-2 text-xs font-bold tracking-wider transition-all
                ${group.combinator === "OR"
                                    ? "bg-amber-700 text-white shadow-inner"
                                    : "bg-transparent text-amber-100/40 hover:bg-amber-800/10"}
              `}
                        >
                            OR
                        </button>
                    </div>

                    <span className="text-xs font-medium text-amber-200/30 uppercase tracking-widest">
                        {group.combinator === "AND" ? "Logic: All must match" : "Logic: Any can match"}
                    </span>
                </div>

                {/* Delete Group Button (only for nested groups) */}
                {isNested && onDelete && (
                    <button
                        onClick={onDelete}
                        className="p-2 text-red-400 hover:bg-red-400/10 rounded-md transition-colors"
                        title="Delete group"
                    >
                        <Trash2 className="w-4 h-4" />
                    </button>
                )}
            </div>

            {/* Conditions */}
            <div className="space-y-2">
                {group.conditions.map((condition, index) => (
                    <div key={index}>
                        {index > 0 && (
                            <div className="text-xs font-medium text-gray-500 my-1 ml-2">
                                {group.combinator}
                            </div>
                        )}
                        <RuleCondition
                            condition={condition}
                            onChange={(c) => handleConditionChange(index, c)}
                            onDelete={() => handleConditionDelete(index)}
                        />
                    </div>
                ))}

                {/* Nested Groups */}
                {group.groups.map((nestedGroup, index) => (
                    <div key={`group-${index}`}>
                        {(index > 0 || group.conditions.length > 0) && (
                            <div className="text-xs font-medium text-amber-500/80 my-2 ml-2">
                                {group.combinator}
                            </div>
                        )}
                        <RuleGroup
                            group={nestedGroup}
                            onChange={(g) => handleNestedGroupChange(index, g)}
                            onDelete={() => handleNestedGroupDelete(index)}
                            depth={depth + 1}
                        />
                    </div>
                ))}
            </div>

            {/* Empty State */}
            {isEmpty && (
                <div className="text-center py-6 text-gray-500 text-sm">
                    No conditions or groups yet. Add a condition or group to get started.
                </div>
            )}

            {/* Action Buttons */}
            <div className="flex gap-2 mt-4">
                <button
                    onClick={handleAddCondition}
                    className="flex items-center gap-2 px-4 py-2 bg-amber-600 text-white rounded-md hover:bg-amber-700 transition-colors text-sm font-medium shadow-sm"
                >
                    <Plus className="w-4 h-4" />
                    Add Condition
                </button>

                {depth < maxDepth && (
                    <button
                        onClick={handleAddGroup}
                        className="flex items-center gap-2 px-4 py-2 bg-amber-900/30 border border-amber-600/30 text-amber-100 rounded-md hover:bg-amber-900/50 transition-colors text-sm font-medium shadow-sm"
                    >
                        <Plus className="w-4 h-4" />
                        Add Group
                    </button>
                )}
            </div>
        </div>
    );
}

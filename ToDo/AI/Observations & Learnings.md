I can see that my best results come fromarchitecture-first prompting. When I clearly define the layer, responsibility boundaries, contracts, and constraints before asking for implementation, the output is clean, production-ready, and aligned with Clean Architecture principles.

I work most effectively with deterministic prompts. When I strictly define input/output contracts, forbid certain behaviors, the responses are significantly more accurate and usable without refactoring.

Prompts That Worked Well

- Architecture → responsibility split → per-file implementation.
- File-scoped prompts with clear assumptions ("assume interfaces exist").
- Zero-shot prompts with strict constraints and no alternatives.
- Prompts that fix DTOs, JSON schema, and output format explicitly.

Prompts That Did Not Work

- Overly broad system-level prompts mixing multiple abstraction levels.
- Prompts with implicit assumptions about token storage or DTO shape.
- Requests combining architecture, implementation, configuration, and frontend integration in a single step.
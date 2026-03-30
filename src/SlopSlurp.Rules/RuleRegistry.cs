using SlopSlurp.Rules.Models;

namespace SlopSlurp.Rules;

public static class RuleRegistry
{
    public static IReadOnlyList<RuleDefinition> AllRules { get; } =
    [
        new("SL001", "Negative Parallelism", "sentence-structure",
            "The \"It's not X -- it's Y\" pattern, often with an em dash. The single most commonly identified AI writing tell. AI uses this to create false profundity by framing everything as a surprising reframe. Includes the causal variant \"not because X, but because Y\" and the cross-sentence reframe where the same noun is negated then repositioned.",
            "\"It's not bold. It's backwards.\""),

        new("SL002", "\"Not X. Not Y. Just Z.\"", "sentence-structure",
            "The dramatic countdown pattern. AI builds tension by negating two or more things before revealing the actual point. Creates a false sense of narrowing down to the truth.",
            "\"Not a bug. Not a feature. A fundamental design flaw.\""),

        new("SL003", "\"The X? A Y.\"", "sentence-structure",
            "Self-posed rhetorical questions answered immediately in the next sentence or clause. The model asks a question nobody was asking, then answers it for dramatic effect.",
            "\"The result? Devastating.\""),

        new("SL004", "Em-Dash Addiction", "formatting",
            "Compulsive overuse of em dashes for dramatic pauses, parenthetical asides and pivot points. A human writer might use 2-3 per piece naturally; AI will use 20+.",
            "\"The problem -- and this is the part nobody talks about -- is systemic.\""),

        new("SL005", "Short Punchy Fragments", "paragraph-structure",
            "Excessive use of very short sentences or sentence fragments as standalone paragraphs for manufactured emphasis. An inhuman style — no real person writes first drafts this way because it doesn't match how humans think or speak.",
            "\"He published this. Openly. In a book. As a priest.\""),

        new("SL006", "Anaphora Abuse", "sentence-structure",
            "Repeating the same sentence opening multiple times in quick succession.",
            "\"They assume that users will pay... They assume that developers will build... They assume that ecosystems will emerge...\""),

        new("SL007", "Tricolon Abuse", "sentence-structure",
            "Overuse of the rule-of-three pattern, often extended to four or five. A single tricolon is elegant; three back-to-back tricolons are a pattern recognition failure.",
            "\"Products impress people; platforms empower them. Products solve problems; platforms create worlds.\""),

        new("SL008", "\"Here's the Kicker\"", "tone",
            "False suspense transitions that promise a revelation but deliver an unremarkable point. Also includes: \"Here's the thing\", \"Here's where it gets interesting\", \"Here's what most people miss\", \"Here's the deal\".",
            "\"Here's the kicker.\""),

        new("SL009", "Bold-First Bullets", "formatting",
            "Every bullet point or list item starts with a bolded phrase or sentence. Almost nobody formats lists this way when writing by hand. A telltale sign of AI-generated documentation and blog posts.",
            "\"Every single bullet point begins with a bold keyword.\""),

        new("SL010", "Fractal Summaries", "composition",
            "\"What I'm going to tell you; what I'm telling you; what I just told you\" — applied at every level of the document. Every subsection gets a summary. Every section gets a summary. The document itself gets a summary.",
            "\"In this section, we'll explore... [3000 words later] ...as we've seen in this section.\""),

        new("SL011", "\"Think of It As...\"", "tone",
            "The patronizing analogy. AI constantly reaches for \"Think of it as...\" or \"It's like a...\" to simplify concepts. The model defaults to teacher mode and assumes the reader needs a metaphor to understand anything.",
            "\"Think of it like a highway system for data.\""),

        new("SL012", "Unicode Decoration", "formatting",
            "Use of unicode arrows (→), smart/curly quotes, and other special characters that can't be easily typed on a standard keyboard. Real writers produce straight quotes and -> or =>.",
            "\"Input → Processing → Output\""),

        new("SL013", "The Dead Metaphor", "composition",
            "Latching onto a single metaphor and beating it into the ground across the entire piece. A human writer would introduce a metaphor, use it then move on. AI will repeat the same metaphor 5-10 times.",
            "\"The ecosystem needs ecosystems to build ecosystem value.\""),

        new("SL014", "Historical Analogy Stacking", "composition",
            "Especially common in technical writing: rapid-fire listing of historical companies or tech revolutions to build false authority.",
            "\"Apple didn't build Uber. Facebook didn't build Spotify. Stripe didn't build Shopify.\""),

        new("SL015", "\"Imagine a World Where...\"", "tone",
            "The classic AI invitation to futurism. To sell the argument, usually begins with \"Imagine\" followed by a list of wonderful things that will happen if the reader agrees.",
            "\"Imagine a world where every tool you use has a quiet intelligence behind it...\""),

        new("SL016", "False Vulnerability", "tone",
            "Simulated self-awareness or honesty that reads as performative. The model pretends to break the fourth wall or admit a bias, creating a false sense of authenticity. Real vulnerability is specific and uncomfortable; AI vulnerability is polished and risk-free.",
            "\"And yes, I'm openly in love with the platform model\""),

        new("SL017", "\"The Truth Is Simple\"", "tone",
            "Asserting that something is obvious, clear or simple instead of actually proving it. Also includes the dramatic reveal: \"but none of them is the real story. The real story is...\" — claiming privileged insight while waving away everything before it.",
            "\"The reality is simpler and less flattering\""),

        new("SL018", "Listicle in a Trench Coat", "paragraph-structure",
            "Numbered or labeled points dressed up as continuous prose. The model writes a listicle but wraps each point in a paragraph starting with \"The first... The second... The third...\" to disguise the format.",
            "\"The first wall is the absence of a free, scoped API... The second wall is the lack of delegated access...\""),

        new("SL019", "One-Point Dilution", "composition",
            "Making a single argument and restating it in 10 different ways across thousands of words. The model pads a simple thesis to feel comprehensive by rephrasing the same idea with different metaphors and framings.",
            "\"The same point, restated eight ways across 4000 words.\""),

        new("SL020", "Grandiose Stakes Inflation", "tone",
            "Everything is the most important thing ever. AI inflates the stakes of every argument to world-historical significance. A blog post about API pricing becomes a meditation on the fate of civilization.",
            "\"This will fundamentally reshape how we think about everything.\""),

        new("SL021", "\"Quietly\" and Other Magic Adverbs", "word-choice",
            "Overuse of \"quietly\" and similar adverbs to convey subtle importance or understated power. Also includes: \"deeply\", \"fundamentally\", \"remarkably\", \"arguably\".",
            "\"quietly orchestrating workflows, decisions, and interactions\""),

        new("SL022", "Content Duplication", "composition",
            "Repeating entire sections or paragraphs verbatim within the same piece. Happens when the model loses track of what it has already written, especially in longer pieces.",
            "\"The same section appeared twice, word-for-word identical.\""),

        new("SL023", "\"Delve\" and Friends", "word-choice",
            "\"Delve\" went from an uncommon English word to appearing in a staggering percentage of AI-generated text. Part of a family of overused AI vocabulary including \"certainly\", \"utilize\", \"leverage\" (as a verb), \"robust\", \"streamline\", and \"harness\".",
            "\"Let's delve into the details...\""),

        new("SL024", "\"Tapestry\" and \"Landscape\"", "word-choice",
            "Overuse of ornate or grandiose nouns where simpler words would do. \"Tapestry\" for anything interconnected. \"Landscape\" for any field or domain. Other offenders: \"paradigm\", \"synergy\", \"ecosystem\", \"framework\".",
            "\"The rich tapestry of human experience...\""),

        new("SL025", "\"It's Worth Noting\"", "sentence-structure",
            "Filler transitions that signal nothing. AI uses these phrases to introduce new points without connecting them to the previous argument. Also includes: \"It bears mentioning\", \"Importantly\", \"Interestingly\", \"Notably\".",
            "\"It's worth noting that this approach has limitations.\""),

        new("SL026", "\"Let's Break This Down\"", "tone",
            "The pedagogical voice that assumes the reader needs hand-holding. AI defaults to a teacher-student dynamic even when writing for expert audiences. Also includes: \"Let's unpack this\", \"Let's explore\", \"Let's dive in\".",
            "\"Let's break this down step by step.\""),

        new("SL027", "The Signposted Conclusion", "composition",
            "Explicitly announcing the conclusion with \"In conclusion\", \"To sum up\", or \"In summary\". Competent writing doesn't need to tell you it's concluding. AI signals its structural moves because it's following a template.",
            "\"In conclusion, the future of AI depends on...\""),

        new("SL028", "Superficial Analyses", "sentence-structure",
            "Tacking a present participle (\"-ing\") phrase onto the end of a sentence to inject shallow analysis that says nothing. Uses phrases like \"highlighting its importance\", \"reflecting broader trends\", or \"contributing to the development of...\".",
            "\"contributing to the region's rich cultural heritage\""),

        new("SL029", "Vague Attributions", "tone",
            "Attributing claims to unnamed authorities instead of being specific. AI invokes \"experts\", \"observers\", \"industry reports\" without naming anyone. Inflates the quantity of sources — presenting what one person said as a widely held view.",
            "\"Experts argue that this approach has significant drawbacks.\""),

        new("SL030", "\"Despite Its Challenges...\"", "composition",
            "The rigid formula where AI acknowledges problems only to immediately dismiss them. Always follows: \"Despite its [positive words], [subject] faces challenges...\" then ends with \"Despite these challenges, [optimistic conclusion].\".",
            "\"Despite these challenges, the initiative continues to thrive.\""),

        new("SL031", "False Ranges", "sentence-structure",
            "Using \"from X to Y\" constructions where X and Y aren't on any real scale. In legitimate use, \"from X to Y\" implies a spectrum with a meaningful middle. AI uses it as a fancy way to list two loosely related things.",
            "\"From innovation to implementation to cultural transformation.\""),

        new("SL032", "Invented Concept Labels", "tone",
            "AI clusters invented compound labels that sound analytical without being grounded. It appends abstract problem-nouns (paradox, trap, creep, divide, vacuum, inversion) to domain words and uses them as if they're established terms. Multiple such labels in the same piece is a strong signal.",
            "\"the supervision paradox\""),

        new("SL033", "The \"Serves As\" Dodge", "word-choice",
            "Replacing simple \"is\" or \"are\" with pompous alternatives like \"serves as\", \"stands as\", \"marks\", or \"represents\". AI avoids basic copulas because its repetition penalty pushes it toward fancier constructions.",
            "\"The building serves as a reminder of the city's heritage.\""),
    ];

    public static IReadOnlyList<RuleDefinition> CodeFirstRules =>
        AllRules.Where(r => r.Id is "SL004" or "SL012").ToList();

    public static IReadOnlyList<RuleDefinition> LlmRules =>
        AllRules.Where(r => r.Id is not ("SL004" or "SL012")).ToList();

    public static RuleDefinition? GetRule(string id) =>
        AllRules.FirstOrDefault(r => r.Id == id);
}

namespace BaseFaq.Models.Faq.Enums;

public enum FaqItemOrigin
{
    Unknown = 0,

    Book = 1,
    Pdf = 2,
    Document = 3,
    Ebook = 4,

    Website = 10,
    WebPage = 11,
    BlogPost = 12,
    OnlineDocumentation = 13,
    WikiPage = 14,

    KnowledgeBase = 20,
    HelpCenter = 21,
    LearningPlatform = 22,

    SocialProfile = 30,
    SocialPost = 31,
    ForumThread = 32,

    Video = 40,
    PodcastEpisode = 41,
    Webinar = 42,

    Repository = 50,
    Readme = 51,
    Issue = 52,

    InternalDocument = 60,
    InternalWiki = 61,

    Email = 70,
    ChatConversation = 71,
}
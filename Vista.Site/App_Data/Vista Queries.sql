SELECT * FROM RSSFeed
SELECT * FROM Items

SELECT RSS.RSSFeedGuid FROM RSSFeed RSS
LEFT OUTER JOIN Items I ON RSS.RSSFeedGuid = I.RSSFeedGuid

DELETE FROM RSSFeed
DELETE FROM Items

SELECT * FROM Items WHERE Guid = 'https://www.bbc.co.uk/news/uk-england-essex-55971315'
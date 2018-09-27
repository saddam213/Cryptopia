
SET IDENTITY_INSERT [dbo].[Currency] ON 
INSERT [dbo].[Currency] (Id, Name, Symbol, TradeFee, MinTradeAmount, MaxTradeAmount, TxFee, WithdrawFee, MinWithdraw, MaxWithdraw, MinConfirmations, [Rank], [Status],IsEnabled)
                  VALUES(1, N'Bitcoin', 'BTC', 0.18, 0.00000001, 100000000, 0.0001, 0.02, 0.001, 100000000,2, 100, 0, 1)
INSERT [dbo].[Currency] (Id, Name, Symbol, TradeFee, MinTradeAmount, MaxTradeAmount, TxFee, WithdrawFee, MinWithdraw, MaxWithdraw, MinConfirmations, [Rank], [Status],IsEnabled)
                   VALUES(2, N'Litecoin', 'LTC', 0.18, 0.00000001, 100000000, 0.0001, 0.02, 10, 100000000,6, 200, 0, 1)
INSERT [dbo].[Currency] (Id, Name, Symbol, TradeFee, MinTradeAmount, MaxTradeAmount, TxFee, WithdrawFee, MinWithdraw, MaxWithdraw, MinConfirmations, [Rank], [Status],IsEnabled)
                   VALUES(3, N'Dogecoin', 'DOGE', 0.18, 0.00000001, 100000000, 0.0001, 0.02, 10, 100000000,6, 300, 0, 1)
INSERT [dbo].[Currency] (Id, Name, Symbol, TradeFee, MinTradeAmount, MaxTradeAmount, TxFee, WithdrawFee, MinWithdraw, MaxWithdraw, MinConfirmations, [Rank], [Status],IsEnabled)
                   VALUES(4, N'Darkcoin', 'DRK', 0.18, 0.00000001, 100000000, 0.0001, 0.02, 10, 100000000,6, 400, 0, 1)
INSERT [dbo].[Currency] (Id, Name, Symbol, TradeFee, MinTradeAmount, MaxTradeAmount, TxFee, WithdrawFee, MinWithdraw, MaxWithdraw, MinConfirmations, [Rank], [Status],IsEnabled)
                   VALUES(5, N'KimDotcoin', 'DOT', 0.18, 0.00000001, 100000000, 0.0001, 0.02, 10, 100000000,6, 400, 0, 1)
SET IDENTITY_INSERT [dbo].[Currency] OFF



--SET IDENTITY_INSERT [dbo].[CurrencyAbout] ON 
--INSERT [dbo].[CurrencyAbout] ([Id], [CurrencyId], [CurrencyData]) VALUES (1, 1, N'<img src="http://fierceandnerdy.com/wp-content/uploads/2012/01/Picard2379.jpg" /><h1>Picard<b>(</b>PCD<b>)</b></h1><p>An accomplished diplomat and tactician, Picard managed to surpass a 22-year career as first officer and later captain of the U.S.S. Stargazer with an even more impressive record as captain of the fleets former flagship U.S.S. Enterprise. In the latter role he not only witnessed the major turning points of recent galactic history but played a major role in them as well, from surviving as the only human abductee of the Borg invasion in 2366, to becoming the chief contact point with the Q Continuum, to serving as arbiter choosing the current ruler of the Klingon Empire and exposing the Romulans as backers of his chief rivals, later helping a pacifist underground movement to gain a toehold there.</p>', N'<h2>Specifications</h2><p>Scrypt Algo - PoW</p><p>500 Picard initial block reward</p><p>Halves every 890000 blocks</p><p>30 Second Block Spacing</p><p>Total of 890,000,000 Picards</p>', N'<h2>Downloads</h2><p>Windows EXE</p><p>Windows SETUP</p><p>Windows Daemon</p><p>Linux Daemon</p><p>Source Code</p>', N'<h2>Social</h2><p>Website</p><p>Twitter</p><p>Facebook</p>')
--INSERT [dbo].[CurrencyAbout] ([Id], [CurrencyId], [CurrencyData]) VALUES (2, 2, N'<img src="http://channelchatter.tv/wp-content/uploads/2014/05/2Blue-geordi.jpg" /><h1>LaForge<b>(</b>LFG<b>)</b></h1><p>In 2365, La Forge was promoted to full lieutenant. He transferred from the command division to the operations division and was named chief engineer. Over the course of the next several years, it became evident that this transfer had been a wise decision. His technical and analytical abilities, his versatility and his cooperative leadership style were very well suited to the requirements of a senior engineering officer aboard a major starship. Just one year later, in 2366, he was promoted to lieutenant commander.</p>', N'<h2>Specifications</h2><p>Scrypt Algo - PoW</p><p>500 LaForge initial block reward</p><p>Halves every 890000 blocks</p><p>30 Second Block Spacing</p><p>Total of 890,000,000 LaForges</p>', N'<h2>Downloads</h2><p>Windows EXE</p><p>Windows SETUP</p><p>Windows Daemon</p><p>Linux Daemon</p><p>Source Code</p>', N'<h2>Social</h2><p>Website</p><p>Twitter</p><p>Facebook</p>')
--INSERT [dbo].[CurrencyAbout] ([Id], [CurrencyId], [CurrencyData]) VALUES (3, 3, N'<img src="http://1.bp.blogspot.com/-QJP5ob22xmw/UYKWaxBz7lI/AAAAAAAAAN4/lgETTOOO3Fw/s1600/data-1335198726.jpg" /><h1>Data<b>(</b>DTA<b>)</b></h1><p>Data served as operations officer and second officer on board the USS Enterprise-D from 2364 until the vessels destruction in 2371. Since he did not require sleep, he routinely stood night watch on the bridge. His quarters were located on deck 2, room 3653. He frequently participated in many of the away missions undertaken. His speed of thought and great strength made him an important asset to the ship, and the fact that he was unaffected by disease, radiation, or mind control was vital on more than one occasion.</p>', N'<h2>Specifications</h2><p>Scrypt Algo - PoW</p><p>500 Data initial block reward</p><p>Halves every 890000 blocks</p><p>30 Second Block Spacing</p><p>Total of 890,000,000 Datas</p>', N'<h2>Downloads</h2><p>Windows EXE</p><p>Windows SETUP</p><p>Windows Daemon</p><p>Linux Daemon</p><p>Source Code</p>', N'<h2>Social</h2><p>Website</p><p>Twitter</p><p>Facebook</p>')
--INSERT [dbo].[CurrencyAbout] ([Id], [CurrencyId], [CurrencyData]) VALUES (4, 4, N'<img src="http://upload.wikimedia.org/wikipedia/it/1/12/Tenente_Worf_-_Star_Trek_TNG.jpg" /><h1>Worf<b>(</b>WRF<b>)</b></h1><p>In 2364, Lieutenant junior grade Worf was assigned as a command division bridge officer on the USS Enterprise, under the command of Captain Jean-Luc Picard. Worf spent most of his first year on the Enterprise-D as a relief officer for the conn and other bridge stations. Worf was permitted a variation from the Starfleet uniform dress code, and wore a Klingon warriors sash, sometimes called a baldric by Humans, over his regular duty uniform.</p>', N'<h2>Specifications</h2><p>Scrypt Algo - PoW</p><p>500 Worf initial block reward</p><p>Halves every 890000 blocks</p><p>30 Second Block Spacing</p><p>Total of 890,000,000 Worfs</p>', N'<h2>Downloads</h2><p>Windows EXE</p><p>Windows SETUP</p><p>Windows Daemon</p><p>Linux Daemon</p><p>Source Code</p>', N'<h2>Social</h2><p>Website</p><p>Twitter</p><p>Facebook</p>')
--INSERT [dbo].[CurrencyAbout] ([Id], [CurrencyId], [CurrencyData]) VALUES (6, 5, N'<img src="http://ben174.github.io/rikeripsum/images/riker.jpg" /><h1>Riker<b>(</b>RKR<b>)</b></h1><p>William Thomas Riker was a noted Starfleet officer, perhaps best known for his long assignment as first officer under Captain Jean-Luc Picard aboard the USS Enterprise-D, and later the USS Enterprise-E. In 2379, he finally accepted a promotion as captain of the USS Titan. In 2361, a transporter accident resulted in two Rikers, with each one being identical to the other, as well as genetically indistinguishable. Their personality and memories were the same up to the point of the duplication. The other Riker eventually decided to use his middle name and became known as Thomas Riker.</p>', N'<h2>Specifications</h2><p>Scrypt Algo - PoW</p><p>500 Riker initial block reward</p><p>Halves every 890000 blocks</p><p>30 Second Block Spacing</p><p>Total of 890,000,000 Rikers</p>', N'<h2>Downloads</h2><p>Windows EXE</p><p>Windows SETUP</p><p>Windows Daemon</p><p>Linux Daemon</p><p>Source Code</p>', N'<h2>Social</h2><p>Website</p><p>Twitter</p><p>Facebook</p>')
--SET IDENTITY_INSERT [dbo].[CurrencyAbout] OFF

SET IDENTITY_INSERT [dbo].[TradePair] ON 

--BTC
INSERT [dbo].[TradePair] (Id, CurrencyId1, CurrencyId2, LastTrade, Change) VALUES(100,2,1, 0.00000000, 0.00)
INSERT [dbo].[TradePair] (Id, CurrencyId1, CurrencyId2, LastTrade, Change) VALUES(101,3,1, 0.00000000, 0.00)
INSERT [dbo].[TradePair] (Id, CurrencyId1, CurrencyId2, LastTrade, Change) VALUES(102,4,1, 0.00000000, 0.00)
INSERT [dbo].[TradePair] (Id, CurrencyId1, CurrencyId2, LastTrade, Change) VALUES(103,5,1, 0.00000000, 0.00)

--LTC
INSERT [dbo].[TradePair] (Id, CurrencyId1, CurrencyId2, LastTrade, Change) VALUES(200,3,2, 0.00000000, 0.00)
INSERT [dbo].[TradePair] (Id, CurrencyId1, CurrencyId2, LastTrade, Change) VALUES(201,4,2, 0.00000000, 0.00)
INSERT [dbo].[TradePair] (Id, CurrencyId1, CurrencyId2, LastTrade, Change) VALUES(202,5,2, 0.00000000, 0.00)

--DRK
INSERT [dbo].[TradePair] (Id, CurrencyId1, CurrencyId2, LastTrade, Change) VALUES(301,3,4, 0.00000000, 0.00)
INSERT [dbo].[TradePair] (Id, CurrencyId1, CurrencyId2, LastTrade, Change) VALUES(302,5,4, 0.00000000, 0.00)

--DOGE
INSERT [dbo].[TradePair] (Id, CurrencyId1, CurrencyId2, LastTrade, Change) VALUES(402,5,3, 0.00000000, 0.00)

--DOT

SET IDENTITY_INSERT [dbo].[TradePair] OFF


--Tradepairs
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (2,1,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (3,1,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (4,1,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (5,1,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (6,1,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (7,1,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (3,2,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (4,2,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (5,2,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (6,2,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (7,2,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (4,3,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (5,3,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (6,3,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (7,3,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (5,4,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (6,4,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (7,4,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (6,5,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (7,5,0.00000000, 0)
GO
INSERT INTO [Cryptopia].[dbo].[TradePair] ([CurrencyId1], [CurrencyId2], [LastTrade], [Change])
VALUES (7,6,0.00000000, 0)
GO

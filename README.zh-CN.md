# TinyWarriorServer

语言：[English](README.md) | 简体中文

## 简介

这是使用 C# 语言为游戏 [TinyWarrior](https://github.com/ZerglingV/TinyWarrior) 所设计的专属服务器。服务器支持多个游戏房间，每个游戏房间 2 至 5 人进行联机游戏，其默认端口为 8765（服务器允许自定义 IP 及端口）。

![TinyWarriorServer.png](TinyWarriorServer/mp.ico)

感谢您的游玩，同时如果在游玩过程中有什么建议或发现什么漏洞欢迎提出。

## 安装

下载发布的 <strong><em>TinyWarriorServer.exe</em></strong> 后直接执行即可进入服务器设置。

## 服务器设置及指令

正式指令（在控制台内输入，不区分大小写）：

| 指令           | 功能                         |
| -------------- | ---------------------------- |
| shutdown       | 关闭服务器                   |
| clear          | 清空控制台                   |
| monitor pause  | 监视器[^1]暂停               |
| monitor resume | 监视器恢复                   |
| show clients   | 显示所有已连接的客户端       |
| remove clients | 移除所有已连接的客户端       |
| show rooms     | 显示所有存在的房间           |
| remove rooms   | 移除所有存在的房间           |
| send           | 向所有已连接的客户端发送消息 |

[^1]: 监视器用来监视玩家连接是否有效，房间中的游戏是否已经结束，其会对无效连接的玩家和游戏结束的房间进行移除，以防止服务器出现无效对象。

测试指令（在控制台内输入，不区分大小写，仅用于测试，正式版本将会删除）：

| 指令   | 功能                       |
| ------ | -------------------------- |
| win    | 使索引为零号的玩家获胜     |
| dead   | 使索引为零号的玩家淘汰     |
| leaver | 使索引为零号的玩家离开游戏 |
| ‘      | 添加用于测试的房间         |

## 服务器截图

<p align="center"><b>自定义IP及端口</b></p>

![TinyWarriorServer.png](TinyWarriorServer/Samples/Sample1.png)

## 版权

部分内容和图片来自网络，版权归原作者或网站所有。 如果有侵犯版权的地方，请联系我删除。

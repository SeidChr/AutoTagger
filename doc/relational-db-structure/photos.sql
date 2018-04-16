/*
Navicat MySQL Data Transfer

Source Server         : Hetzner
Source Server Version : 50558
Source Host           : 78.46.178.185:3306
Source Database       : instatagger

Target Server Type    : MYSQL
Target Server Version : 50558
File Encoding         : 65001

Date: 2018-04-16 23:11:11
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for photos
-- ----------------------------
DROP TABLE IF EXISTS `photos`;
CREATE TABLE `photos` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `largeUrl` text NOT NULL,
  `thumbUrl` text NOT NULL,
  `shortcode` varchar(50) NOT NULL,
  `likes` int(11) NOT NULL,
  `comments` int(11) NOT NULL,
  `user` varchar(50) NOT NULL,
  `follower` int(11) NOT NULL,
  `following` int(11) NOT NULL,
  `posts` int(11) NOT NULL,
  `uploaded` timestamp NULL DEFAULT NULL,
  `created` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`,`shortcode`),
  UNIQUE KEY `id` (`id`),
  UNIQUE KEY `imgId` (`shortcode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

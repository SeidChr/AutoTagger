/*
Navicat MySQL Data Transfer

Source Server         : Hetzner
Source Server Version : 50558
Source Host           : 78.46.178.185:3306
Source Database       : instatagger

Target Server Type    : MYSQL
Target Server Version : 50558
File Encoding         : 65001

Date: 2018-04-08 15:43:17
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for photos
-- ----------------------------
DROP TABLE IF EXISTS `photos`;
CREATE TABLE `photos` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `imgUrl` text NOT NULL,
  `imgId` varchar(50) NOT NULL,
  `instaUrl` text NOT NULL,
  `likes` int(11) NOT NULL,
  `comments` int(11) NOT NULL,
  `follower` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8;

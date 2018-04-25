/*
Navicat MySQL Data Transfer

Source Server         : Hetzner
Source Server Version : 50558
Source Host           : 78.46.178.185:3306
Source Database       : instatagger

Target Server Type    : MYSQL
Target Server Version : 50558
File Encoding         : 65001

Date: 2018-04-19 00:01:37
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for itags
-- ----------------------------
DROP TABLE IF EXISTS `itags`;
CREATE TABLE `itags` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(30) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `posts` int(11) NOT NULL,
  `updated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`,`name`),
  UNIQUE KEY `id` (`id`) USING BTREE,
  UNIQUE KEY `name` (`name`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;

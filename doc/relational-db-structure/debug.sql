/*
Navicat MySQL Data Transfer

Source Server         : Hetzner
Source Server Version : 50560
Source Host           : 78.46.178.185:3306
Source Database       : instatagger

Target Server Type    : MYSQL
Target Server Version : 50560
File Encoding         : 65001

Date: 2018-04-24 17:20:57
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for debug
-- ----------------------------
DROP TABLE IF EXISTS `debug`;
CREATE TABLE `debug` (
  `id` int(11) NOT NULL,
  `source` text NOT NULL,
  `query` text NOT NULL,
  `result` text NOT NULL,
  `created` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

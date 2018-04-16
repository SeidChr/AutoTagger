/*
Navicat MySQL Data Transfer

Source Server         : Hetzner
Source Server Version : 50558
Source Host           : 78.46.178.185:3306
Source Database       : instatagger

Target Server Type    : MYSQL
Target Server Version : 50558
File Encoding         : 65001

Date: 2018-04-16 23:04:57
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for photo_itag_rel
-- ----------------------------
DROP TABLE IF EXISTS `photo_itag_rel`;
CREATE TABLE `photo_itag_rel` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `photoId` int(11) NOT NULL,
  `itagId` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `photoId` (`photoId`),
  KEY `itagId` (`itagId`),
  CONSTRAINT `photo_itag_rel_ibfk_2` FOREIGN KEY (`itagId`) REFERENCES `itags` (`id`),
  CONSTRAINT `photo_itag_rel_ibfk_1` FOREIGN KEY (`photoId`) REFERENCES `photos` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
